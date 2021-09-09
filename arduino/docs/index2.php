<?php 
session_start();
if(!isset($_SESSION['access']) || $_SESSION['access']!=true){
header("location:index.php");}
else{ ?>

<html>
<head>
<title>HOME</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<script src="//code.jquery.com/jquery-1.11.0.min.js"></script>

 <link rel="stylesheet" type="text/css" href="style.css">
 <script>  


        function show()  
        {  
            $.ajax({  
                url: "transfer/temp-1.php",  
                cache: false,  
                success: function(html){  
                    $("#content").html(html); 
				}
             }); 
			 $.ajax({  
                url: "transfer/temp-1-time.php",  
                cache: false,  
                success: function(html){  
                    $("#time").html(html); 
				}
             }); 
           $.ajax({  
                url: "transfer/temp-2.php",  
                cache: false,  
                success: function(html){  
                    $("#content-1").html(html); 
                }
             }); 
			 $.ajax({  
                url: "transfer/temp-3.php",  
                cache: false,  
                success: function(html){  
                    $("#content-4").html(html); 
                }
             }); 
			 
			 $.ajax({  
                url: "transfer/fire.php",  
                cache: false,  
                success: function(html){  
                    $("#content-5").html(html); 
                }
             }); 
			 
			 $.ajax({  
                url: "transfer/move.php",  
                cache: false,  
                success: function(html){  
                    $("#content-6").html(html); 
                }
             }); 
             
             $.ajax({  
                url: "transfer/ledstate.php",  
                cache: false,  
                success: function(html){  
                    $("#content-3").html(html); 
                }
             }); 
             
        }
        
        $(document).ready(function(){  
            show();  
            setInterval('show()',500);  
        }); 
        
       function check()
       {
        var inp = document.getElementsByName("status");
        for (var i = 0; i < inp.length; i++) 
		{
          if (inp[i].type == "radio" && inp[i].checked) 
		  {
            alert("selected: " + inp[i].value);
          }
        }
       }

      function AjaxFormRequest(result_id,led,url) 
	  {
		var inp = document.getElementsByName("status");
        if ((inp[0]).checked || (inp[1]).checked)
		{

			jQuery.ajax({
            url:     url,
            type:     "POST",
            dataType: "html",
            data: jQuery("#"+led).serialize(),
           });	
		  }	
       else 
        {
          alert("Выберите состояние!");	
        }	
		
      }
	  

</script>
    
    
    
</head>
    <body>
          <div class="r">
          <p class="r1">Температура  дома</p>
          <div class="r2" style="display:inline-block;">
          <div class="r3" id="content"></div> 
          <div class="r3"> C<sup>o</sup></div>
		  <div class="r1" id="time"></div> 
          </div>
          </div>
		  
		  <div class="r">
          <p class="r1">Влажность</p>
          <div class="r2" style="display:inline-block;">
          <div class="r3" id="content-1"></div> 
          <div class="r3"> % </div>
          </div>
          </div>
        

          <div class="r">
          <p class="r1">Загазованность</p>
          <div class="r2" >
          <div class="r3" id="content-4"></div> 
          </div>
          </div>
		  
		  <div class="r">
          <p class="r1">Котел</p>
          <div class="r2" >
          <div class="r3" id="content-5"></div> 
          </div>
          </div>
		  
		  <div class="r">
          <p class="r1">Движение</p>
          <div class="r2" >
          <div class="r3" id="content-6"></div> 
          </div>
          </div>
		 
          
          <div class="r">
          <div class="rl">
          <p class="r1">Выключатель</p>
          <div class="r2" style="font-size:35px" >
          <form  id="led" action="" method="post"  >
          <label><input type="radio" name="status" value="1"> ON </label>
          <label><input type="radio" name="status" value="0"> OFF </label>
          <br>
          <input class="submitButton" type="submit" value="Отправить" onclick="AjaxFormRequest('messegeResult', 'led', 'transfer/led.php')" >
          </form>
          </div>
          </div>
          
          <div class="rr">
          <p class="r1">Состояние</p>
          <div class="r2"style="font-size:35px" >
          <div class="r3" id="content-3"></div> 
          </div>
          </div>
		  </div>
		  
		  
          
         
         

        
        
   </body>
</html> 
<?php } ?>