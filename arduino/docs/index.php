<?php
  if(!empty($_POST['paswd'])){
     $pass = "090479";
    if($_POST['paswd']==$pass){
      session_start();
      $_SESSION['access']=true;
      header("Location: index2.php") ;
    }
    else {
       header("Location: error.php") ;
    }
  }
  else
  {
    ?>
	<p align="center"><font size="3">Введите пароль</font></p>
 <center>
    <form method="POST">
      <input type="text" name="paswd">
      <input type="submit">
    </form>
</center>
    <?php
  }
?>