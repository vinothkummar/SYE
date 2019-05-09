// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


jQuery("[data-showwhen-questionid]").each(function (s, e) {

    var $this = jQuery(this);
    $this.hide();

    var questionToCheck = $this.data("showwhen-questionid");
    var valToCheck = $this.data("showwhen-value").toString();

    jQuery("[name=" + questionToCheck + "]").on("change", function () {
        var answer = jQuery(this).val();
        showWhen($this, answer, valToCheck);
    });

});

function showWhen($this, answer, valToCheck) {
    if (valToCheck !== "" && answer === valToCheck) {
        $this.show();
    } else {
        $this.hide();
    }
}