function indexViewModel(){
    var self = this;
    self.uploadSessionData = ko.observable();

    self.uploadSessionNotification = function () {
        $.ajax({
            type: "get",
            datatype: "json",
            contenttype: "application/json",
            url: "/t/UploadSessionNotification",
            success: function (data) {
                if (data.status === "HAS") {
                    self.uploadSessionData(data);
                    $("#uploadSessionNotification").modal("show");
                }
            }
        });
    };
};

var ivm = new indexViewModel();
ko.applyBindings(ivm);
$(document).ready(function () {
    ivm.uploadSessionNotification();
    $(function () {
        $('[data-toggle="popover"]').popover()
    })
})