function sidebar() {
   
    var temp=document.getElementById('sidebar');

    var dissewtting=temp.style.display;

    if(dissewtting==='block')
    {
        temp.style.display='none';
        document.getElementById('data').style.width="100vw";

    }
    else
    {
        temp.style.display='block';
   
        document.getElementById('data').style.width="85vw";
    }
     
}
function displayFilename() {
    
    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;
    
}
function openTab(evt, cityName) {
    evt.preventDefault();
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";
}