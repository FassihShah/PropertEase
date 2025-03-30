
const connection = new signalR.HubConnectionBuilder().withUrl("/messagingHub").build();

connection.on("NewMessageReceived", function(senderName, propertyTitle) {

    const toastHtml = `
        <div id="messageToast" class="toast show position-fixed bottom-0 end-0 m-3" role="alert" aria-live="assertive" aria-atomic="true" 
             style="z-index: 1050; cursor: pointer;">
            <div class="toast-header text-white" style="background-color: #ff7525;">
                <strong class="me-auto">New Message</strong>
                <small>Just now</small>
                <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                New message about <strong>${propertyTitle}</strong> from <strong>${senderName}</strong>.
            </div>
        </div>`;

    $("body").append(toastHtml);

    // Redirect to Received Messages when clicked
    $(document).on("click", "#messageToast", function () {
        window.location.href = "/Message/ReceivedMessages";
    });
});

connection.start();

document.getElementById("sendMessageButton").addEventListener("click", function () {

    const receiverId = document.getElementById("receiverId").value;
    const senderName = document.getElementById("senderName").value;
    const propertyTitle = document.getElementById("propertyTitle").value;

    connection.invoke("NotifyNewMessage", receiverId, senderName, propertyTitle);
})