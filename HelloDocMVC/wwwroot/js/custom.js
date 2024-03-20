

//File Upload In Forms

function displayFilename() {

    var input = document.getElementById('myFile');
    var output = document.getElementById('selectedFilename');
    output.textContent = input.files[0].name;

}


//Contact field in forms

//const phoneInputField = document.querySelector("#phone");
//const phoneInput = window.intlTelInput(phoneInputField, {
//    utilsScript:
//        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
//});
//const phoneInputField1 = document.querySelector("#phone1");
//const phoneInput1 = window.intlTelInput(phoneInputField1, {
//    utilsScript:
//        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
//});

