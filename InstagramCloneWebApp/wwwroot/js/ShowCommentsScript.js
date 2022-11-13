var comments = document.getElementsByName("commentSection");

function HideCommentsOnLoad() {
    for (let i = 0; i < comments.length; i++)
    {
        comments[i].style.display = "none";
    }
}

function ShowCommentSection(id) {
    let x = document.getElementsByName(id);

    if (x[0].style.visibility == "visible") {
        x[0].style.position = "absolute";
        x[0].style.visibility = "hidden";
    }
    else {
        x[0].style.position = "relative";
        x[0].style.visibility = "visible";
    }
}