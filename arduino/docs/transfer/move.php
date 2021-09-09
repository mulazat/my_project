<?php 
$myFile6 = "../txt/move.txt";
$fh6 = fopen($myFile6, 'r');
if(filesize($myFile6) > 0)
{

$theData6 = fread($fh6, filesize($myFile6));
fclose($fh6);

if  ( $theData6 == 1)
{
 echo " <p class='on'>ON</p>";
}

if  ( $theData6 == 0)
{
 echo "<p class='off'>OFF</p>";
}
} else {
	echo "<p class='off'>ERR</p>";
}

?>