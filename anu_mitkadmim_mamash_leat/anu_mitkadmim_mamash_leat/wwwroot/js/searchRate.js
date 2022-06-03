$(function () {
    $('form').submit(e => {
        e.preventDefault();
        const q = $('#searchRate').val();
        $('tbody').load('/Ratings/Search2?query='+q);
    })
});