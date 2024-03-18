// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $(".t-tab > .btn > .rounded").click(function () {
        $(".t-tab > .btn > .rounded").removeClass("active");
        $(this).addClass("active");
    });

    $(".t-filter > .btn ").click(function () {
        console.log(123)
        $(".t-filter> .btn").removeClass("filterborder");
        $(this).addClass("filterborder");
    });
});