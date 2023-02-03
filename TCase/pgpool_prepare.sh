#!/bin/bash
ip_suac=10.0.0.25
pgconf=/etc/postgresql/11/main/postgresql.conf
pgpoolconf=/etc/pgpool2/pgpool.conf
errmsg="Введите trust, md5 либо pam"
stands=( sudcm sufs susrv )
KEY_PATH=/var/lib/postgresql/.ssh/id_rsa
if [ $(hostname -I | awk '{print $1}') != $ip_suac ]; then
    echo "Ошибка! Исполнять на suac"
    exit 1
fi
case $1 in

        trust)
            auth_type=trust
            ;;
        md5)
            auth_type=md5
            ;;
        pam)
            auth_type=pam
            ;;
        *) 
            echo $errmsg
            exit 1
            ;;
    
esac


apt -y install pgpool2 rsync sshpass iputils-arping
pgpconf="
backend_hostname0 = '10.0.0.22'\n\
backend_port0 = 5432\n\
backend_weight0 = 1\n\
backend_data_directory0 = '/var/lib/postgresql/11/main'\n\
backend_flag0 = 'ALLOW_TO_FAILOVER'\n\
backend_application_name0 = 'sufs'\n\
backend_hostname1 = '10.0.0.23'\n\
backend_port1 = 5432\nbackend_weight1 = 1\n\
backend_data_directory1 = '/var/lib/postgresql/11/main'\n\
backend_flag1 = 'ALLOW_TO_FAILOVER'\n\
backend_application_name1 = 'susrv'
"

echo '#!/bin/bash
# ID упавшего узла
FAILED_NODE=$1
# IP нового мастера
NEW_MASTER=$2
# Путь к триггерному файлу
TRIGGER_FILE=$3

KEY_PATH=/var/lib/postgresql/.ssh/id_rsa
if [ $FAILED_NODE = 1 ];
then
    echo "Ведомый сервер $FAILED_NODE вышел из строя"
    exit 1
fi

echo "Ведущий сервер $FAILED_NODE вышел из строя"
echo "Новый ведущий сервер: $NEW_MASTER"

ssh -T -i $KEY_PATH postgres@$NEW_MASTER touch $TRIGGER_FILE
exit 0' > /etc/pgpool2/failover.sh
chmod 755 /etc/pgpool2/failover.sh

echo '#!/bin/bash
ATTACHED_NODE_ID=$1
ATTACHED_HOST=$2
ATTACHED_PORT=$3
ATTACHED_DATA=$4

NEW_MASTER_ID=$5
NEW_MASTER_HOST=$6
NEW_MASTER_PORT=$7
NEW_MASTER_DATA=$8

OLD_MASTER_ID=$9
OLD_PRIMARY_ID=$10
OLD_PRIMARY_HOST=$11
OLD_PRIMARY_PORT=$12

SSH_USER=root
KEY_PATH=/root/.ssh/id_rsa
PGCTL="/usr/bin/pg_ctlcluster"
KEY_PATH_POST=/var/lib/postgresql/.ssh/id_rsa

if [ $ATTACHED_NODE_ID = 1 ];
    then	
        echo "!!! FAILBACK START !!!"
        echo "ВНИМАНИЕ: подключается нода $ATTACHED_HOST [id:$ATTACHED_NODE_ID] [id:$ATTACHED_DATA] "
        echo "Старый ведущий сервер $OLD_MASTER_HOST [id:$OLD_MASTER_ID]"
        echo "Новый ведущий сервер $NEW_MASTER_HOST [id:$NEW_MASTER_ID]"
        ssh -i $KEY_PATH $SSH_USER@$ATTACHED_HOST "sudo service postgresql stop"
        ssh -i $KEY_PATH $SSH_USER@$NEW_MASTER_HOST "sudo -u postgres psql -c \"SELECT pg_start_backup('\''stream'\'');\""
        ssh -i $KEY_PATH $SSH_USER@$NEW_MASTER_HOST "sudo rsync -v -a $ATTACHED_DATA/ $ATTACHED_HOST:$ATTACHED_DATA/ --exclude postmaster.pid"
        ssh -i $KEY_PATH $SSH_USER@$NEW_MASTER_HOST "sudo -u postgres psql -c \"SELECT pg_stop_backup();\""
        ssh -i $KEY_PATH $SSH_USER@$ATTACHED_HOST "sudo service postgresql start"
        echo "!!! FAILBACK FINISH !!!"
    else
        echo "Ведущий сервер $NEW_MASTER_HOST вышел из строя. Верните его вручную"
fi' > /etc/pgpool2/failback.sh
chmod 755 /etc/pgpool2/failback.sh


check_resolv() {
    ping -q -c 1 $1 > /dev/null 2>&1
    if [ $? = 0 ]
    then
        echo "хост $1 доступен"
    else
        echo "хост $1 не доступен"
        exit 1
    fi
}

if [ ! -f /root/.ssh/id_rsa ]; then
    ssh-keygen -q -f /root/.ssh/id_rsa -N "" > /dev/null
fi
if [ ! -f /var/lib/postgresql/.ssh/id_rsa ]; then
    sudo -u postgres ssh-keygen -q -f '/var/lib/postgresql/.ssh/id_rsa' -N ""
fi
echo "postgres:12345678" | chpasswd
pg_md5 -m 12345678 -f $pgpoolconf -u postgres
echo 'postgres:'`pg_md5 12345678` > /etc/pgpool2/pcp.conf
cp /usr/share/doc/pgpool2/examples/pgpool.pam /etc/pam.d/

sed -i "s/listen_addresses = .*/listen_addresses = '*'/" $pgpoolconf
sed -i "s/sr_check_period = 0/sr_check_period = 10/" $pgpoolconf
sed -i "s/sr_check_user = 'nobody'/sr_check_user = 'postgres'/" $pgpoolconf
sed -i "s/sr_check_password = ''/sr_check_password = '12345678'/" $pgpoolconf
sed -i "s/health_check_period = 0/health_check_period = 5/" $pgpoolconf
sed -i "s/health_check_max_retries = 0/health_check_max_retries = 5/" $pgpoolconf
sed -i "s/health_check_retry_delay = 1/health_check_retry_delay = 3/" $pgpoolconf
sed -i "s/health_check_user = 'nobody'/health_check_user = 'postgres'/" $pgpoolconf
sed -i "s/health_check_password = ''/health_check_password = '12345678'/" $pgpoolconf
sed -i "s/failover_command = ''/failover_command = '\/etc\/pgpool2\/failover.sh %d %H \/var\/lib\/postgresql\/11\/main\/failover'/" $pgpoolconf
sed -i "s/failback_command = ''/failback_command = '\/etc\/pgpool2\/failback.sh %d %h %p %D %m %H %r %R %M %P %N %S'/" $pgpoolconf
sed -i "s/search_primary_node_timeout = 300/search_primary_node_timeout = 30/" $pgpoolconf
sed -i "s/recovery_user = 'nobody'/recovery_user = 'postgres'/" $pgpoolconf
sed -i "s/recovery_password = ''/recovery_password = '12345678'/" $pgpoolconf
sed -i 's/load_balance_mode = off/load_balance_mode = on/' $pgpoolconf
sed -i 's/enable_pool_hba = off/enable_pool_hba = true/' $pgpoolconf
sed -i 's/master_slave_mode = off/master_slave_mode = on/' $pgpoolconf
sed -i "/Backend Connection/,/Authentication/!b;//!d;/Backend Connection/a\\${pgpconf}" $pgpoolconf 
echo -e "local all all ${auth_type}\nhost all all 127.0.0.1/32 ${auth_type}\nhost all all 10.0.0.21/32 ${auth_type}\nhost all all 10.0.0.25/32 ${auth_type}\nhost all all 10.0.0.22/32 trust\nhost all all 10.0.0.23/32 trust\nhost all all 10.0.0.100/32 ${auth_type}" > /etc/pgpool2/pool_hba.conf
sed -i "s/use_watchdog = off/use_watchdog = on/" $pgpoolconf
sed -i "s/wd_hostname = ''/wd_hostname = '10.0.0.25'/" $pgpoolconf
sed -i "s/delegate_IP = ''/delegate_IP = '10.0.0.100'/" $pgpoolconf
sed -i "s/$_IP_$\/24/$_IP_$\/19/" $pgpoolconf
sed -i "s/heartbeat_destination0 = 'host0_ip1'/heartbeat_destination0 = '10.0.0.21'/" $pgpoolconf
sed -i "s/wd_lifecheck_user = 'nobody'/wd_lifecheck_user = 'postgres'/" $pgpoolconf
sed -i "s/wd_lifecheck_password = ''/wd_lifecheck_password = '12345678'/" $pgpoolconf
sed -i "s/#other_pgpool_hostname0 = 'host0'/other_pgpool_hostname0 = '10.0.0.21'/" $pgpoolconf
sed -i "s/#other_pgpool_port0 = 5432/other_pgpool_port0 = 5432/" $pgpoolconf
sed -i "s/#other_wd_port0 = 9000/other_wd_port0 = 9000/" $pgpoolconf
sed -i "s/memory_cache_enabled = off/memory_cache_enabled = on/" $pgpoolconf
sed -i "s/arping_path = '\/usr\/sbin'/arping_path = '\/usr\/bin'/" $pgpoolconf
sed -i "s/usr\/sbin\/arping/usr\/bin\/arping/" $pgpoolconf
sed -i "s/var\/log\/pgpool\/oiddir/var\/log\/postgresql\/oiddir/" $pgpoolconf
sed -i "s/enable_consensus_with_half_votes = off/enable_consensus_with_half_votes = on/" $pgpoolconf



for (( i=0;i<3;i=i+1 )) ; do
    check_resolv ${stands[i]}

    COMM="
    spawn ssh-copy-id root@${stands[i]} -f
    expect \"yes/no\" {send \"yes\r\"}
    expect \"password\" {send \"1\r\"}
    expect eof
    "
    expect -c "$COMM"

    

    case ${stands[i]} in
    sudcm)
        
        ssh root@sudcm "\
        apt -y install pgpool2 rsync sshpass postgresql-client-11 iputils-arping;
        cat /dev/zero | ssh-keygen -q -f /root/.ssh/id_rsa -N \"\";
        sed -i 's/#   StrictHostKeyChecking ask/    StrictHostKeyChecking no/' /etc/ssh/ssh_config;
        sshpass -p 1 ssh-copy-id -i /root/.ssh/id_rsa.pub root@10.0.0.25 -f;
        sshpass -p 1 ssh-copy-id -i /root/.ssh/id_rsa.pub root@10.0.0.22 -f;
        sshpass -p 1 ssh-copy-id -i /root/.ssh/id_rsa.pub root@10.0.0.23 -f;
        systemctl restart ssh;
        systemctl restart sshd;
        "
        scp -p -r /etc/pgpool2/* root@sudcm:/etc/pgpool2/
        ssh root@sudcm "\
        sed -i \"s/listen_addresses = '10.0.0.25'/listen_addresses = '10.0.0.21'/\" /etc/pgpool2/pgpool.conf;
        sed -i \"s/wd_hostname = '10.0.0.25'/wd_hostname = '10.0.0.21'/\" /etc/pgpool2/pgpool.conf;
        sed -i \"s/heartbeat_destination0 = '10.0.0.21'/heartbeat_destination0 = '10.0.0.25'/\" /etc/pgpool2/pgpool.conf;
        sed -i \"s/other_pgpool_hostname0 = '10.0.0.21'/other_pgpool_hostname0 = '10.0.0.25'/\" /etc/pgpool2/pgpool.conf;
        cp /usr/share/doc/pgpool2/examples/pgpool.pam /etc/pam.d/
        "
        ;;
    sufs)
        ssh root@sufs "\
        apt -y install postgresql-11 postgresql-11-pgpool2 rsync sshpass;
        cat /dev/zero | ssh-keygen -q -f /root/.ssh/id_rsa -N \"\";
        sed -i 's/#   StrictHostKeyChecking ask/    StrictHostKeyChecking no/' /etc/ssh/ssh_config;
        sshpass -p 1 ssh-copy-id -i /root/.ssh/id_rsa.pub root@10.0.0.23 -f;
        systemctl restart ssh;
        systemctl restart sshd;
        sed -i \"s/listen_addresses = .*/listen_addresses = '10.0.0.22'/\" /etc/postgresql/11/main/postgresql.conf;
        sed -i 's/#wal_level = replica/wal_level = hot_standby/' /etc/postgresql/11/main/postgresql.conf;
        sed -i 's/#max_wal_senders = 10/max_wal_senders = 10/' /etc/postgresql/11/main/postgresql.conf;
        sed -i 's/#wal_keep_segments = 0/wal_keep_segments = 32/' /etc/postgresql/11/main/postgresql.conf;
        echo -e 'local all postgres trust\nhost replication postgres 10.0.0.0/24 trust\nhost all all 10.0.0.25/32 trust\nhost all all 10.0.0.21/32 trust' > /etc/postgresql/11/main/pg_hba.conf;
        service postgresql restart;
        "
        ;;  
        
    susrv)
        ssh root@susrv "\
        apt -y install postgresql-11 postgresql-11-pgpool2 rsync sshpass;
        cat /dev/zero | ssh-keygen -q -f /root/.ssh/id_rsa -N \"\";
        sed -i 's/#   StrictHostKeyChecking ask/    StrictHostKeyChecking no/' /etc/ssh/ssh_config;
        sshpass -p 1 ssh-copy-id -i /root/.ssh/id_rsa.pub root@10.0.0.22 -f;
        systemctl restart ssh;
        systemctl restart sshd;
        service postgresql stop;
        echo -e 'local all postgres trust\nhost replication postgres 10.0.0.0/24 trust\nhost all all 10.0.0.25/32 trust\nhost all all 10.0.0.21/32 trust' > /etc/postgresql/11/main/pg_hba.conf;
        sed -i \"s/listen_addresses = .*/listen_addresses = '10.0.0.23'/\" /etc/postgresql/11/main/postgresql.conf;
        sed -i 's/#hot_standby = on/hot_standby = on/' /etc/postgresql/11/main/postgresql.conf;
        "
        ;;
    esac

    COMM="
    spawn ssh root@${stands[i]} sudo passwd postgres
    expect \":\" {send \"12345678\r\"}
    expect \":\" {send \"12345678\r\"}
    expect eof
    "
    expect -c "$COMM"


    COMM="
    spawn sudo -u postgres ssh-copy-id postgres@${stands[i]} -f
    expect \"yes/no\" {send \"yes\r\"}
    expect \"password\" {send \"12345678\r\"}
    expect eof
    "
    expect -c "$COMM"

done
ssh root@sudcm "\
sudo -u postgres sshpass -p 12345678 ssh-keygen -q -N \"\" -f /var/lib/postgresql/.ssh/id_rsa <<<y;
sudo -u postgres sshpass -p 12345678 ssh-copy-id -i /var/lib/postgresql/.ssh/id_rsa.pub postgres@10.0.0.25 -f;
sudo -u postgres sshpass -p 12345678 ssh-copy-id -i /var/lib/postgresql/.ssh/id_rsa.pub postgres@10.0.0.22 -f;
sudo -u postgres sshpass -p 12345678 ssh-copy-id -i /var/lib/postgresql/.ssh/id_rsa.pub postgres@10.0.0.23 -f;
"
ssh root@sufs "\
sudo -u postgres sshpass -p 12345678 ssh-keygen -q -N \"\" -f /var/lib/postgresql/.ssh/id_rsa <<<y;
sudo -u postgres sshpass -p 12345678 ssh-copy-id -i /var/lib/postgresql/.ssh/id_rsa.pub postgres@10.0.0.23 -f;
sudo -u postgres psql -c \"SELECT pg_start_backup('stream');\" > /dev/null 2>&1;
sshpass -p 1 rsync -v -a /var/lib/postgresql/11/main/ 10.0.0.23:/var/lib/postgresql/11/main/ --exclude postmaster.pid;
sudo -u postgres psql -c \"SELECT pg_stop_backup();\" > /dev/null 2>&1;
"

ssh root@susrv "\
sudo -u postgres sshpass -p 12345678 ssh-keygen -q -N \"\" -f /var/lib/postgresql/.ssh/id_rsa <<<y;
sudo -u postgres sshpass -p 12345678 ssh-copy-id -i /var/lib/postgresql/.ssh/id_rsa.pub postgres@10.0.0.22 -f;
echo -e \"standby_mode = 'on'\nprimary_conninfo = 'host=10.0.0.22 port=5432 user=postgres'\ntrigger_file = 'failover'\" > /var/lib/postgresql/11/main/recovery.conf;
chown postgres.postgres /var/lib/postgresql/11/main/recovery.conf;
service postgresql start;  
"
ssh root@sudcm "\
service pgpool2 stop && rm -rf /var/log/postgresql/pgpool_status
"
service pgpool2 stop && rm -rf /var/log/postgresql/pgpool_status && service pgpool2 start
ssh root@sudcm "\
service pgpool2 start
"

echo "Done"
