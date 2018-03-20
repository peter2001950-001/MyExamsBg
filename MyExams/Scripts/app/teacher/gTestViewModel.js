function gTestViewModel() {
    var self = this;
    self.test = ko.observable();

    self.getGTest = function () {
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            url: "/t/GetGTest?id=" + gTestId,
            success: function (data) {
                if (data.status === "OK") {
                    self.test(data.test);
                }
            }
        });
    }
}
var gtvm = new gTestViewModel();
$(document).ready(function(){
    gtvm.getGTest();
});
ko.applyBindings(gtvm);