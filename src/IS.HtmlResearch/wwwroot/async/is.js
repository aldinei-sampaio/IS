"use strict";

class AssetLoader {
    static GetCommonImage(name) {
        return "../img/" + name + ".png";
    }
    static GetBackground(name) {
        return "../img/" + name + ".jpg";
    }
    static GetPersonSprite(name, emotion) {
        return "../sprites/" + name + "_" + emotion + ".png"
    }
    static GetEmotionSprite(name) {
        return "../sprites/" + name + ".png";
    }
}

class Task {
    static async WaitAll() {
        for (var n in arguments)
            await arguments[n];
    }
    static async Delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}

class Background {
    constructor(div) {
        this.div = div;
        this.position = "left";
    }

    Show(imageName) {
        this.div.css("background-image",  "url('" + AssetLoader.GetBackground(imageName) + "')");
    }

    async FadeInAsync(imageName, direction) {
        var me = this;

        var clone = this.div.clone();
        clone.css("background-image", "url(" + AssetLoader.GetBackground(imageName) + "')");
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
        newBack.css("background-image", "url(" + AssetLoader.GetBackground(imageName) + ")");
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

    async ShowChoicesAsync(options, clickCallBack) {
        var optionsContainer = $('<div class="choices"></div>');

        var internalContainer = $('<div class="scroll"></div>')
            .css("left", "-100%")
            .css("opacity", "0")
            .appendTo(optionsContainer);

        var currentPage = 0;

        if (options.choices.length > 5) {
            if (options.current !== void 0) {
                for (var n = 0; n < options.choices.length; n++) {
                    var option = options.choices[n];
                    if (option.key === options.current) {
                        currentPage = Math.floor(n / 4);
                        break;
                    }
                }
            }
            ShowCurrentPage(internalContainer);
        } else {
            var validButton = false;
            options.choices.forEach(choice => {
                if (AppendChoice(choice, internalContainer)) {
                    validButton = true;
                }
            });

            if (!validButton) {
                throw new Error("Nenhuma op��o desbloqueada informada.");
            }
        }

        optionsContainer.appendTo(this.balloon);
        await internalContainer.animate({ opacity: 1, left: 0 }, 500).promise();

        function ShowCurrentPage(container) {
            var startIndex = currentPage * 4;
            var count = Math.min(4, options.choices.length - startIndex);

            for (var n = 0; n < count; n++) {
                var option = options.choices[startIndex + n];
                AppendChoice(option, container);
            }

            while (count < 4) {
                $('<div class="navigation"></div>').appendTo(container);
                count++;
            }

            var navBar = $(
                '<div class="navigation">' +
                '<button class="previous">' +
                '<img class="normal" src="' + AssetLoader.GetCommonImage("previous") + '" />' +
                '<img class="hover" src="' + AssetLoader.GetCommonImage("previous_hover") + '" />' +
                '</button>' +
                '<button class="next">' +
                '<img class="normal" src="' + AssetLoader.GetCommonImage("next") + '" />' +
                '<img class="hover" src="' + AssetLoader.GetCommonImage("next_hover") + '" />' +
                '</button>' +
                '</div>'
            );
            navBar.appendTo(container);

            navBar.find("button.next").on("click", async () => {
                currentPage++;
                if (currentPage * 4 >= options.choices.length) {
                    currentPage = 0;
                }

                var newContainer = $('<div class="scroll"></div>')
                    .css("left", "100%")
                    .css("opacity", "0")
                    .appendTo(optionsContainer);

                ShowCurrentPage(newContainer);

                await Task.WaitAll(
                    internalContainer.animate({ opacity: 0, left: "-100%" }, 250).promise(),
                    newContainer.animate({ opacity: 1, left: 0 }, 250).promise()
                );

                internalContainer.remove();
                internalContainer = newContainer;
            });

            navBar.find("button.previous").on("click", async () => {
                currentPage--;
                if (currentPage < 0) {
                    currentPage = (options.choices.length - 1) / 4;
                }

                var newContainer = $('<div class="scroll"></div>')
                    .css("left", "-100%")
                    .css("opacity", "0")
                    .appendTo(optionsContainer);

                ShowCurrentPage(newContainer);

                await Task.WaitAll(
                    internalContainer.animate({ opacity: 0, left: "100%" }, 250).promise(),
                    newContainer.animate({ opacity: 1, left: 0 }, 250).promise()
                );

                internalContainer.remove();
                internalContainer = newContainer;
            });
        }

        function AppendChoice(choice, container) {
            var button = CreateButton(choice);
            button.find(".textContainer span").text(choice.text);
            button.appendTo(container);
            if (choice.type === ChoiceType.LOCKED) {
                return false;
            }
            button.on("click", async () => {
                await internalContainer.animate({ opacity: 0, left: "-100%" }, 250).promise();
                optionsContainer.remove();
                clickCallBack(choice);
                return true;
            });
            return true;
        }

        function CreateButton(choice) {
            switch (choice.type) {
                case ChoiceType.ICON:
                    return $('<button class="choiceButton icon">' +
                        '<div class="textContainer">' +
                        '<span></span>' +
                        '</div>' +
                        '<div class="iconContainer">' +
                        '<img src="' + AssetLoader.GetCommonImage(choice.imageName) + '" />' +
                        '</div>' +
                        '</button>');
                case ChoiceType.LOCKED:
                    return $('<button class="choiceButton locked">' +
                        '<div class="textContainer">' +
                        '<span></span>' +
                        '</div>' +
                        '<div class="iconContainer">' +
                        '<img src="' + AssetLoader.GetCommonImage("locked") + '" />' +
                        '</div>' +
                        '</button>');
                case ChoiceType.UNLOCKED:
                    return $('<button class="choiceButton unlocked">' +
                        '<div class="textContainer">' +
                        '<span></span>' +
                        '</div>' +
                        '<div class="iconContainer">' +
                        '<img src="' + AssetLoader.GetCommonImage("unlocked") + '" />' +
                        '</div>' +
                        '</button>');
                default:
                    return $('<button class="choiceButton normal">' +
                        '<div class="textContainer">' +
                        '<span></span>' +
                        '</div>' +
                        '</button>');
            }
        }
    }
}

const Emotion = {
    NORMAL: "normal",
    HAPPY: "happy",
    SAD: "sad",
    SURPRISED: "surprised",
    ANGRY: "angry"
};

const ChoiceType = {
    SIMPLE: "simple",
    ICON: "icon",
    LOCKED: "locked",
    UNLOCKED: "unlocked"
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
        this.textContainer.removeClass("thought");
    }

    ShowThought() {
        this.arrowContainer.html(this.thoughtArrowHtml);
        this.textContainer.addClass("thought");
    }

    async ChangeEmotionAsync(emotion) {
        const me = this;

        const emotionImage = AssetLoader.GetEmotionSprite(emotion);
        const personImage = AssetLoader.GetPersonSprite(this.person, emotion);

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

            await Task.WaitAll(
                newEmotion.animate({ opacity: 1 }, 1000).promise(),
                newPerson.animate({ opacity: 1 }, 1000).promise()
            );

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
        await Task.Delay(2000);
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
        await Task.Delay(2000);
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

        await Task.WaitAll(
            dummyText.animate({ height: textHeight + "px" }, 250).promise(),
            dummy.animate({ opacity: 1, width: "100%", left: "0%" }, 250).promise()
        );

        await TypeWriter.WriteAsync(dummyText, text);

        this.balloon.css("opacity", "1");
        dummy.remove();
    }

    async HideAsync() {
        var textHeight = this.textContainer.height();
        await this.textContainer.animate({ opacity: 0 }, 250).promise();

        this.textContainer
            .css("height", textHeight + "px")
            .css("display", "inline-block")
            .empty();

        await Task.WaitAll(
            this.textContainer.animate({ height: "10px" }, 250).promise(),
            this.balloon.animate({ opacity: 0, width: "10%", left: "50%" }, 250).promise()
        );

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

class TextInputBalloon extends CenterBalloon {
    CreateScene() {
        return $(
            '<div class="scene">' +
                '<div class="tutorialContainer">' +
                    '<div class="balloon center">' +
                        '<div class="balloonText withoutTitle">' +
                            '<span></span>' +
                            '<div class="balloonInput">' +
                                '<input type="text" maxlength="16" spellcheck="false" />' +
                                '<img src="' + AssetLoader.GetCommonImage("return_disabled") + '" />' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>'
        );
    }

    async ShowAsync(container, text, defaultText, callback) {
        await super.ShowAsync(container, "", text);

        var input = this.scene.find(".balloonInput input");
        var img = this.scene.find(".balloonInput img");

        input.on("input propertychange paste", UpdateImage);
        input.on('keyup', e => {
            if (e.key === 'Enter' || e.keyCode === 13) {
                OnReturn();
            }
        });

        input.val(defaultText);
        input.focus();
        input.select();

        UpdateImage();
        img.on("click", OnReturn);

        function UpdateImage() {
            var value = input.val().trim();
            if (value === "") {
                img.attr("src", AssetLoader.GetCommonImage("return_disabled"));
            } else {
                img.attr("src", AssetLoader.GetCommonImage("return"));
            }
        }

        var me = this;

        async function OnReturn() {
            var value = TitleCase(input.val().trim());
            if (value !== "") {
                await me.HideAsync();
                callback(value);
            }
        }

        function TitleCase(str) {
            var splitStr = str.toLowerCase().split(' ');
            for (var i = 0; i < splitStr.length; i++) {
                splitStr[i] = splitStr[i].charAt(0).toUpperCase() + splitStr[i].substring(1).toLowerCase();
            }
            return splitStr.join(' ');
        }
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

        await trophy.animate({ opacity: 0.9, top: "0" }, 1000).promise();
        await Task.Delay(4000);
        await trophy.animate({ opacity: 0, top: "-" + height + "px" }, 1000).promise();

        trophy.remove();
    }
}