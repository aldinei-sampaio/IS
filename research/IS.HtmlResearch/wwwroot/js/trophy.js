class Trophy {
    static Show(container, title, text) {
        container.find(".trophy").remove();
        var trophy = $(
            '<div class="trophy">' +
                '<div class="trophy_image"></div>' +
                '<div class="trophy_body">' +
                    '<div class="trophy_title">' + title + '</div>' +
                    '<div class="trophy_text">' + text + '</div>' +
                '</div>' +
            '</div>'
        ).appendTo(container);

        setTimeout(function () {
            CssHelper.AddOutClass(trophy);
        }, 5000);
    }
}