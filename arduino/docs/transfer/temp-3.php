<?php 
$myFile4 = "../txt/in-3.txt";
$fh4 = fopen($myFile4, 'r');
$theData4 = fread($fh4, filesize($myFile4));
fclose($fh4);
echo $theData4;
?>