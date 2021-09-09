#include <dht11.h>
#include <SPI.h>
#include <Ethernet.h>
dht11 DHT;
#define DHT11_PIN 7
int movPin = 8;
const int outPin9 = 9;

byte mac[] = { 0x54, 0x34, 0x41, 0x30, 0x30, 0x33 };
//IPAddress ip(192,168,10,10);
//IPAddress myDns(8,8,8,8);
byte myDns[] = { 192, 168, 10, 1 };
byte ip[] = { 192, 168, 10, 30 };
byte gateway[] = { 192, 168, 10, 1 };
byte subnet[] = { 255, 255, 255, 0 };
EthernetClient client;
char server[] = "home.cs20710"; // РёРјСЏ РІР°С€РµРіРѕ СЃРµСЂРІРµСЂР°  www.arduino.ru

int buff=0;
int h;                                    // Р—РЅР°С‡РµРЅРёРµ С‚РµРјРїРµСЂР°С‚СѓСЂС‹
int t;  
int counterFailed = 0;

void setup()
{
  Serial.begin(9600);
  Ethernet.begin(mac, ip, myDns, gateway);
  //Ethernet.begin(mac);
  //Serial.print("My IP address: ");
  Serial.println(Ethernet.localIP());
  delay(1000);
  pinMode(movPin, INPUT);
  pinMode(outPin9, OUTPUT);
  digitalWrite (outPin9, HIGH);

}
void loop()
 {
  int val = digitalRead(movPin);
    Serial.println(val);
    delay(100);
  int fire = 0; //РїРёРЅ РЅР° РєРѕС‚РѕСЂРѕРј РїРѕРґРєР»СЋС‡РµРЅ Р”Р°С‚С‡РёРє РїР»Р°РјРµРЅРё
  int smoke_gas = 1; //РїРёРЅ РЅР° РєРѕС‚РѕСЂРѕРј РїРѕРґРєР»СЋС‡РµРЅ MQ-2
  int sensorFire = analogRead(fire);
  int sensorGas = analogRead(smoke_gas);
  int chk;
  chk = DHT.read(DHT11_PIN);
  t = DHT.temperature;
  h = DHT.humidity;
 Serial.println("connecting...");

  if (client.connect(server, 80)) {
    Serial.println("connected");
    Serial.println("---------------");
    //Serial.println(t);
    //Serial.println(h);
    //Serial.println(sensorGas);
    //Serial.println(sensorFire);
    Serial.print("My IP address: ");
    Serial.println(Ethernet.localIP());
    
    client.print("GET /add_data.php?");
    client.print("paswd=*****************************"); // РЎРїРµС†РёР°Р»СЊРЅС‹Р№ РєРѕРґ, РЅР°РїСЂРёРјРµСЂ asREb25C
    client.print("&");
    client.print("temperature=");
    client.print(t);
    client.print("&");
    client.print("temperature1=");
    client.print(h);
    client.print("&");
    client.print("gas=");
    client.print(sensorGas);
    client.print("&");
    client.print("fire=");
    client.print(sensorFire);
    client.print("&");
    client.print("move=");
    client.print(val);
    client.println(" HTTP/1.1");
    client.print("HOST: ");
    client.println(server);
    client.println("Connection: close");
    client.println();
    client.println();
    client.stop();
    delay(500);
    client.flush();
    Serial.print("GET /add_data.php?");
    Serial.print("temperature=");
    Serial.print(t);
    Serial.print("&");
    Serial.print("temperature1=");
    Serial.print(h);
    Serial.print("&");
    Serial.print("gas=");
    Serial.print(sensorGas);
    Serial.print("&");
    Serial.print("fire=");
    Serial.print(sensorFire);
    Serial.print("&");
    Serial.print("move=");
    Serial.print(val);
    Serial.println(" HTTP/1.1");
    Serial.print("HOST: ");
    Serial.println(server);
    Serial.println("Connection: close");
    delay(100);
    } 
  else {
        Serial.println("connection failed");
        counterFailed=counterFailed+1;
        
       }
   delay(10000);
   if (counterFailed > 3) {
    digitalWrite(outPin9, LOW);
    counterFailed=0;
    delay (2000);
    digitalWrite (outPin9, HIGH);
    delay (50000);
   }
}
    

