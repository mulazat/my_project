
<?php
	    
		if(!empty($_GET['paswd']))
		{
        $pass = "090479";
        if($_GET['paswd']==$pass)
		{

		$S1 =  $_GET['temperature'];
	    $myFile1 = "txt/in-1.txt";
		$fh1 = fopen($myFile1, 'w') or die("can't open file");
		fwrite($fh1, $S1);
		fclose($fh1);
		
        $S3 =  $_GET['temperature1'];
	    $myFile3 = "txt/in-2.txt";
		$fh3 = fopen($myFile3, 'w') or die("can't open file");
		fwrite($fh3, $S3);
		fclose($fh3);
		
		$S4 =  $_GET['gas'];
	    $myFile4 = "txt/in-3.txt";
		$fh4 = fopen($myFile4, 'w') or die("can't open file");
		fwrite($fh4, $S4);
		fclose($fh4);
		
		$S5 =  $_GET['fire'];
	    $myFile5 = "txt/fire.txt";
		$fh5 = fopen($myFile5, 'w') or die("can't open file");
		fwrite($fh5, $S5);
		fclose($fh5);
		
		$S6 =  $_GET['move'];
	    $myFile6 = "txt/move.txt";
		$fh6 = fopen($myFile6, 'w') or die("can't open file");
		fwrite($fh6, $S6);
		fclose($fh6);
      
        $myFile = "txt/out-1.txt";
        $fh = fopen($myFile, 'r');
        $theData = fread($fh, filesize($myFile));
        fclose($fh);
        echo $theData;
		}
		}
        if ($S5 > 1000)
          {
	         mail("mulazat@gmail.com", "Котел", "Котел выключен!"); 
          }
	    if ($S1 > 1000)
          {
	         mail("mulazat@gmail.com", "Температура", "Температура критическая"); 
          }	
        if ($S6 > 0)
          {
	         mail("mulazat@gmail.com", "Движение", "Обнаружено движение!"); 
        }
	
?>

