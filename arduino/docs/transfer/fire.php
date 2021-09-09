<?php 
$myFile5 = "../txt/fire.txt";
$fh5 = fopen($myFile5, 'r');
$theData5 = fread($fh5, filesize($myFile5));
fclose($fh5);
echo $theData5;
?>