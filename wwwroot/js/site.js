function ReloadPage(id) {
    setTimeout(() => {
        const msg = document.getElementById(id);
        if (msg) location.reload();
    }, 2000);
};
