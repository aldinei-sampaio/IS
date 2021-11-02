function TimeoutAsync(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

class Background {
    constructor(div) {
        this.div = div;
        this.position = "left";
    }

    Show(imageName) {
        this.div.css("background-image", "url('../img/" + imageName + ".jpg')");
    }

    async FadeInAsync(imageName, direction) {
        var me = this;

        var clone = this.div.clone();
        clone.css("background-image", "url('../img/" + imageName + ".jpg')");
        clone.css("opacity", "0");

        if (direction === "left") {
            clone.css("left", "-5%");
        } else if (direction === "right") {
            clone.css("left", "-95%");
        }

        clone.insertAfter(this.div);

        await clone.animate({ opacity: "1.0" }, 500, "linear").promise();

        if (direction === "left") {
            me.Left();
        } else if (direction === "right") {
            me.Right();
        }
        me.Show(imageName);
        clone.remove()
    }

    async TransitionateAsync(imageName, color, direction) {
        var me = this;

        var transition = this.div.clone();
        transition.css("background-image", "");
        transition.css("background-color", color);
        transition.css("opacity", "0");
        transition.insertAfter(this.div);

        var newBack = this.div.clone();
        newBack.css("background-image", "url('../img/" + imageName + ".jpg')");
        newBack.css("opacity", "0");
        newBack.insertAfter(transition);

        if (direction === "left") {
            newBack.css("left", "-5%");
        } else if (direction === "right") {
            newBack.css("left", "-95%");
        }

        await transition.animate({ opacity: "1.0" }, 250, "linear").promise();
        await newBack.animate({ opacity: "1.0" }, 250, "linear").promise();

        if (direction === "left") {
            me.Left();
        } else if (direction === "right") {
            me.Right();
        }

        me.Show(imageName);
        newBack.remove();
        transition.remove();
    }

    Left() {
        this.div.css("left", "-5%");
        this.position = "left";
    }

    Right() {
        this.div.css("left", "-95%");
        this.position = "right";
    }

    async PanoramicAsync() {
        if (this.position === "left") {
            this.position = "right";
            await this.div.animate({ left: "-95%" }, 2000).promise();
        } else {
            this.position = "left";
            await this.div.animate({ left: "-5%" }, 2000).promise();
        }
    }

    async ProtagAsync() {
        var newPosition = (this.position === "left") ? "0%" : "-90%";
        await this.div.animate({ left: newPosition }, 500).promise();
    }

    async InterlAsync() {
        var newPosition = (this.position === "left") ? "-10%" : "-100%";
        await this.div.animate({ left: newPosition }, 500).promise();
    }

    async DefaultAsync() {
        var newPosition = (this.position === "left") ? "-5%" : "-95%";
        await this.div.animate({ left: newPosition }, 500).promise();
    }
}

class TypeWriter {
    static TypeDelay = 3;

    static async WriteAsync(container, text) {
        container.empty();

        var currentElement = $("<span />").appendTo(container);
        var spans = [];

        var temp = $("<span>" + text + "</span>");
        temp.each(function () {
            ProcessChild(currentElement, $(this), spans);
        });

        function ProcessChild(currentElement, element, spans) {
            element.contents().each(function () {
                if (this.nodeType == 3) {
                    var contents = $(this).text();

                    var newSpans = $('<span>' + contents.split(' ').join('</span> <span>') + '</span>');
                    newSpans.hide().appendTo(currentElement).each(function (i) {
                        var span = $(this);
                        span.css({ display: 'inline', opacity: 0 });
                        spans.push(span);
                    });
                } else {
                    var tagName = $(this).prop("tagName").toLowerCase();
                    var child = $("<" + tagName + " />");
                    child.appendTo(currentElement);
                    ProcessChild(child, $(this), spans);
                }
            });
        }

        for (var i in spans) {
            await spans[i].animate({ opacity: 1 }, this.TypeDelay).promise();
        }
    }
}

class Balloon {
    constructor() {
        this.scene = void 0;
        this.balloon = void 0;
        this.textContainer = void 0;
        this.titleContainer = void 0;
    }

    ChangeTitle(title) {
        if (this.titleContainer !== void 0) {
            this.titleContainer.html(title);
        }
    }

    async ShowTextAsync(text) {
        if (this.balloon === void 0 || this.textContainer === void 0) {
            return;
        }

        var isBalloonEmpty = (this.textContainer.html() === "");

        var dummy = this.balloon.clone();
        if (isBalloonEmpty) {
            dummy.css("opacity", "0");
        }
        var dummyText = dummy.find(".balloonText")
            .html("")
            .css("height", this.textContainer.height());
        dummy.insertAfter(this.balloon);

        if (isBalloonEmpty) {
            await dummy.animate({ opacity: 1 }, 100).promise();
        }

        this.balloon.css("opacity", "0");
        this.textContainer.html(text);

        var newHeight = this.textContainer[0].offsetHeight;
        if (newHeight !== dummyText[0].offsetHeight) {
            await dummyText.animate({ height: newHeight }, 100).promise();
        }

        await TypeWriter.WriteAsync(dummyText, text);
        this.balloon.css("opacity", "1");
        dummy.remove();
    }
}

const Emotion = {
    NORMAL: "normal",
    HAPPY: "happy",
    SAD: "sad",
    SURPRISED: "surprised",
    ANGRY: "angry"
};

class PersonBalloon extends Balloon {
    constructor(directionClass, imageClass) {
        super();
        this.balloon = void 0;
        this.emotion = Emotion.NORMAL;
        this.directionClass = directionClass;
        this.imageClass = imageClass;
        this.voiceArrowHtml = void 0;
        this.thoughtArrowHtml = void 0;
        this.arrowContainer = void 0;
    }

    async ShowAsync(container, person, title) {
        this.person = person;

        this.scene = $(
            '<div class="scene">' +
            '<div class="personContainer">' +
            '<div class="personSizer ' + this.directionClass + '">' +
            '</div>' +
            '</div>' +
            '<div class="dialogContainer">' +
            '<div class="balloon ' + this.directionClass + '">' +
            '<div class="balloonTitle">' +
            '<span>' + title + '</span>' +
            '</div>' +
            '<svg class="balloonArrow ' + this.directionClass + '"></svg>' +
            '<div class="balloonText withTitle"><span></span></div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );

        this.titleContainer = this.scene.find(".balloonTitle span");
        this.personContainer = this.scene.find(".personSizer");
        this.balloon = this.scene.find(".dialogContainer");
        this.arrowContainer = this.scene.find(".balloonArrow");
        this.textContainer = this.scene.find(".balloonText span");
        this.ChangeEmotionAsync(this.emotion);
        this.scene.appendTo(container);
    };

    ShowVoice() {
        this.arrowContainer.html(this.voiceArrowHtml);
    }

    ShowThought() {
        this.arrowContainer.html(this.thoughtArrowHtml);
    }

    async ChangeEmotionAsync(emotion) {
        const me = this;

        const emotionImage = "../sprites/" + emotion + ".png";
        const personImage = "../sprites/" + this.person + "_" + emotion + ".png";

        if (me.currentEmotion === void 0) {
            me.currentEmotion = $('<img class="' + this.imageClass + '" />').appendTo(me.personContainer);
            me.currentPerson = $('<img class="' + this.imageClass + '" />').appendTo(me.personContainer);
        }

        if (me.emotion === emotion) {
            me.currentEmotion.attr("src", emotionImage);
            me.currentPerson.attr("src", personImage);
        } else {
            var newEmotion = me.currentEmotion
                .clone()
                .attr("src", emotionImage)
                .css("opacity", "0")
                .insertAfter(me.currentEmotion);
            var newPerson = me.currentPerson
                .clone()
                .attr("src", personImage)
                .css("opacity", "0")
                .insertAfter(me.currentPerson);

            var p1 = newEmotion.animate({ opacity: 1 }, 1000).promise();
            var p2 = newPerson.animate({ opacity: 1 }, 1000).promise();

            await p1;
            await p2;

            me.currentEmotion.attr("src", emotionImage);
            me.currentPerson.attr("src", personImage);

            newEmotion.remove();
            newPerson.remove();
        }

        me.emotion = emotion;
    }
}

class LeftPersonBalloon extends PersonBalloon {
    constructor() {
        super("left", "");
        this.voiceArrowHtml = '<path d="M 0 28 L 15 0 L 15 28" />';
        this.thoughtArrowHtml = '<ellipse cx="14" cy="5" rx="4" ry="2" /><ellipse cx="11" cy="11" rx="6" ry="3" /><ellipse cx="8" cy="19" rx="8" ry="4" />';
    }

    async ShowAsync(container, person, title) {
        super.ShowAsync(container, person, title);
        this.scene.css("opacity", "0");
        this.scene.css("left", "-50%");
        this.scene.css("top", "15%");
        await this.scene.animate({ opacity: "1", left: "0", top: "0" }, 250).promise();
    }

    async HideAsync() {
        await this.scene.animate({ opacity: "0", left: "-50%", top: "15%" }, 250).promise();
        this.scene.remove();
        this.scene = void 0;
    }

    async ShowBonusAsync(text, cssClass) {
        var bonus = $('<div class="attribute ' + cssClass + ' ' + this.directionClass + '">' + text + '</div>')
            .css("opacity", "0")
            .css("bottom", "-20%")
            .appendTo(this.personContainer);

        await bonus.animate({ opacity: 1, bottom: 0 }, 1000).promise();
        await TimeoutAsync(2000);
        await bonus.animate({ opacity: 0, scale: 0, left: "25%", bottom: "25%" }, 1000).promise();
        bonus.remove();
    }
}

class RightPersonBalloon extends PersonBalloon {
    constructor() {
        super("right", "flipHorizontally");
        this.voiceArrowHtml = '<path d="M 0 28 L 0 0 L 15 28" />';
        this.thoughtArrowHtml = '<ellipse cx="6" cy="5" rx="4" ry="2" /><ellipse cx="9" cy="11" rx="6" ry="3" /><ellipse cx="12" cy="19" rx="8" ry="4" />';
    }

    async ShowAsync(container, person, title) {
        super.ShowAsync(container, person, title);
        this.scene.css("opacity", "0");
        this.scene.css("right", "-50%");
        this.scene.css("top", "15%");
        await this.scene.animate({ opacity: "1", right: "0", top: "0" }, 250).promise();
    }

    async HideAsync() {
        await this.scene.animate({ opacity: "0", right: "-50%", top: "15%" }, 250).promise();
        this.scene.remove();
        this.scene = void 0;
    }

    async ShowBonusAsync(text, cssClass) {
        var bonus = $('<div class="attribute ' + cssClass + ' ' + this.directionClass + '">' + text + '</div>')
            .css("opacity", "0")
            .css("bottom", "-20%")
            .appendTo(this.personContainer);

        await bonus.animate({ opacity: 1, bottom: 0 }, 1000).promise();
        await TimeoutAsync(2000);
        await bonus.animate({ opacity: 0, transform: "scale(0,0)", right: "25%", bottom: "25%" }, 1000).promise();
        bonus.remove();
    }
}

class CenterBalloon extends Balloon {
    CreateScene() {
    }

    async ShowAsync(container, title, text) {
        this.scene = this.CreateScene();

        this.titleContainer = this.scene.find(".balloonTitle span");
        this.balloon = this.scene.find(".tutorialContainer");
        this.textContainer = this.scene.find(".balloonText span");

        var dummy = this.balloon.clone();

        this.scene.css("opacity", "1");
        this.balloon.css("opacity", "0");
        this.textContainer.html(text);
        this.titleContainer.html(title);
        this.scene.appendTo(container);

        var textHeight = this.textContainer.height();

        dummy.find(".balloonTitle span").html(title);
        dummy.css("width", "10%");
        dummy.css("left", "50%");
        dummy.css("opacity", "0");
        dummy.insertAfter(this.balloon);

        var dummyText = dummy.find(".balloonText span");

        var p1 = dummyText.animate({ height: textHeight + "px" }, 250);
        var p2 = dummy.animate({ opacity: 1, width: "100%", left: "0%" }, 250).promise();

        await p1;
        await p2;

        await TypeWriter.WriteAsync(dummyText, text);

        this.balloon.css("opacity", "1");
        dummy.remove();
    }

    async HideAsync() {
        this.textContainer.css("height", this.textContainer.height());
        await this.textContainer.animate({ opacity: 0 }, 250).promise();
        await this.balloon.animate({ opacity: 0, width: "10%", left: "50%", height: 0 }, 250).promise();
        this.scene.remove();
        this.scene = void 0;
    }
}

class TutorialBalloon extends CenterBalloon {
    CreateScene() {
        return $(
            '<div class="scene">' +
            '<div class="tutorialContainer">' +
            '<div class="balloon center">' +
            '<div class="balloonTitle">' +
            '<span></span>' +
            '</div>' +
            '<div class="balloonText withTitle"><span></span></div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );
    }
}

class NarrationBalloon extends CenterBalloon {
    CreateScene() {
        return $(
            '<div class="scene">' +
            '<div class="tutorialContainer">' +
            '<div class="balloon center">' +
            '<div class="balloonText withoutTitle"><span></span></div>' +
            '</div>' +
            '</div>' +
            '</div>'
        );
    }
}

class Trophy {
    static async ShowAsync(container, title, text) {
        var trophy = $(
            '<div class="trophy">' +
            '<div class="trophy_image"></div>' +
            '<div class="trophy_body">' +
            '<div class="trophy_title">' + title + '</div>' +
            '<div class="trophy_text">' + text + '</div>' +
            '</div>' +
            '</div>'
        )
            .css("opacity", "0")
            .css("top", "-30%")
            .appendTo(container);

        var height = trophy[0].offsetHeight;

        trophy.css("top", "-" + height + "px");

        await trophy.animate({ opacity: 0.8, top: "0" }, 1000).promise();
        await TimeoutAsync(4000);
        await trophy.animate({ opacity: 0, top: "-" + height + "px" }, 1000).promise();

        trophy.remove();
    }
}