class Background {
    constructor(div) {
        this.div = div;
        this.position = "left";
    }

    Show(imageName) {
        this.div.css("background-image", "url('img/" + imageName + ".jpg')");
    }

    FadeIn(imageName) {
        var me = this;

        var clone = this.div.clone();
        clone.css("background-image", "url('img/" + imageName + ".jpg')");
        clone.css("opacity", "0");
        clone.insertAfter(this.div);
        clone.animate({ opacity: "1.0" }, 500, function () {
            me.Show(imageName);
            clone.remove()
        });
    }

    Transitionate(imageName, color) {
        var me = this;

        var transition = this.div.clone();
        transition.css("background-image", "");
        transition.css("background-color", color);
        transition.css("opacity", "0");
        transition.insertAfter(this.div);

        var newBack = this.div.clone();
        newBack.css("background-image", "url('img/" + imageName + ".jpg')");
        newBack.css("opacity", "0");
        newBack.insertAfter(transition);

        transition.animate({ opacity: "1.0" }, 500, function () {
            newBack.animate({ opacity: "1.0" }, 500, function () {
                me.Show(imageName);
                newBack.remove();
                transition.remove();
            })
        })
    }

    Left() {
        this.div.css("left", "-5%");
        this.position = "left";
    }

    Right() {
        this.div.css("right", "-95%");
        this.position = "right";
    }

    Panoramic() {
        if (this.position === "left") {
            this.position = "right";
            this.div.animate({ left: "-95%" }, 2000);
        } else {
            this.position = "left";
            this.div.animate({ left: "-5%" }, 2000);
        }
    }

    Protag() {
        var newPosition = (this.position === "left") ? "0%" : "-90%";
        this.div.animate({ left: newPosition }, 500);
    }

    Interl() {
        var newPosition = (this.position === "left") ? "-10%" : "-100%";
        this.div.animate({ left: newPosition }, 500);
    }

    Default() {
        var newPosition = (this.position === "left") ? "-5%" : "-95%";
        this.div.animate({ left: newPosition }, 500);
    }
}