<?php 
$myFile = "../txt/in-1.txt";
if (file_exists($myFile)) {
	echo "Время:" . date ("F d Y H:i:s.", filemtime ($myFile));
}
?>