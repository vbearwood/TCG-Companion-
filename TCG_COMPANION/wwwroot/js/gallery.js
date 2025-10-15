function gallery () {
    let idx = 0;
    const cardsPerPage = 6;
    const gallery = document.getElementsByClassName("Gallery");

    showImages(idx);

    window.changeImage = function(n) {
        showImages(idx += n);
    }

    function showImages(n) {
        const totalCards = gallery.length;

        if (n >= totalCards) {
            idx = 0; 
        } else if (n < 0) {
            idx = totalCards - cardsPerPage; 
        }

        for (let i = 0; i < totalCards; i++) {
            gallery[i].style.display = "none";
        }

        for (let i = idx; i < idx + cardsPerPage && i < totalCards; i++) {
            gallery[i].style.display = "block";
        }
    }
};
