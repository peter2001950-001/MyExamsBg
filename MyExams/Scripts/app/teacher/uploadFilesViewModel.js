$('#uploadFiles').on('change', function (e) {
    var files = e.target.files;
    if (files.length > 0) {
       if (window.FormData !== undefined) {
           var data = new FormData();
           for (var x = 0; x < files.length; x++){
               data.append("file" + x, files[x]);
           }

           $.ajax({
               type: "POST",
               url: '/t/UploadFilesProcess',
               contentType: false,
               processData: false,
               data: data,
               beforeSend: function () {
                   $("#progressBar").css("display", "block");
                   $("#progressBar").width("0%");
               },
               xhr: function () {
                   var myXhr = $.ajaxSettings.xhr();
                   if (myXhr.upload) {
                       myXhr.upload.addEventListener('progress', progress, false);
                   }
                   return myXhr;
               },
               success: function (result) {
                   if (result.status == "OK")
                   {
                       uploadViewModel.IsUploadView(false);
                       localStorage.setItem("sessionId", result.sessionId);
                       uploadViewModel.startPull();
                   };
               },
               error: function (xhr, status, p3, p4){
                   var err = "Error " + " " + status + " " + p3 + " " + p4;
                   if (xhr.responseText && xhr.responseText[0] == "{")
                       err = JSON.parse(xhr.responseText).Message;
                       console.log(err);
                    }
                });
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
          }
     }
});

function progress(e) {

    if (e.lengthComputable) {
        var max = e.total;
        var current = e.loaded;

        var Percentage = Math.round((current * 100) / max);
        $("#progressBar").width(Percentage + "%");
        $("#progressBar").html(Percentage + "%");
        
    }
}

function UploadViewModel() {
    var self = this;
    self.IsUploadView = ko.observable(true);
    self.percentage = ko.observable("0%");

    self.startPull = function () {
        var interval = function () {
            $.ajax({
                type: "GET",
                url: '/t/UploadSessionPull',
                dataType: "json",
                contentType: "application/json",
                data: { sessionIdentifier: localStorage.getItem("sessionId") },
                success: function (result) {
                    if (result.status == "OK") {
                        
                        self.percentage(result.percentage);
                        if (result.percentage = "100%") {
                            clearInterval(interval);
                            localStorage.removeItem("sessionId");
                            setTimeout(function () {

                                window.location.href = "/t/"
                            }, 3000);
                        }
                    };
                }
            });
        }
        setInterval(interval, 1000);
    }
}
var uploadViewModel = new UploadViewModel();
ko.applyBindings(uploadViewModel);