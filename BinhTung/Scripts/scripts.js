function IndexJs() {
    $(".banner-owl").owlCarousel({
        items: 1,
        dots: false,
        nav: false,
        loop: true,
        //animateIn: "fadeIn",
        //animateOut: "fadeOut",
        //smartSpeed: 10,
        navText: ["<i class='fal fa-angle-left'></i>", "<i class='fal fa-angle-right'></i>"],
        autoplay: true
    });

    $(".partner").owlCarousel({
        margin: 20,
        autoplay: true,
        //lazyLoad: true,
        responsiveClass: true,
        responsive: {
            0: {
                margin: 10,
                items: 2
            },
            600: {
                margin: 15,
                items: 3
            },
            1000: {
                items: 3
            }
        }
    });

    $(".feedback-owl").owlCarousel({
        margin: 20,
        autoplay: true,
        dots: false,
        loop: true,
        nav: false,
        items: 1
    });
}


$(function () {

    $(".bars-mobile").on("click", function () {
        $(this).find("i").toggleClass("fa-bars").toggleClass("fa-times");
        $("body").toggleClass("show-menu");
    });

    window.addEventListener("scroll", function () {
        var header = document.querySelector(".header-bottom");
        header.classList.toggle("sticky", window.scrollY > 0);
    });

    AOS.init();

    $("#contact_form").on("submit",
        function (e) {
            e.preventDefault();
            $.post("/Home/ContactForm",
                $(this).serialize(),
                function (data) {
                    if (data.status) {
                        $.toast({
                            heading: 'Gửi liên hệ thành công',
                            text: 'Chúng tôi sẽ liên hệ lại với bạn sớm nhất có thể!!',
                            //icon: 'success',
                        })
                        $("#contact_form").trigger("reset");
                    } else {
                        $.toast({
                            heading: 'Gửi thất bại',
                            text: 'Vui lòng nhập đúng định dạng',
                            icon: 'error',
                        })
                    }
                });
        });

    var $inputItem = $(".js-inputWrapper");
    $inputItem.length && $inputItem.each(function () {
        var $this = $(this),
            $input = $this.find(".formRow--input"),
            placeholderTxt = $input.attr("placeholder"),
            $placeholder;

        $input.after('<span class="placeholder">' + placeholderTxt + "</span>"),
            $input.attr("placeholder", ""),
            $placeholder = $this.find(".placeholder"),

            $input.val().length ? $this.addClass("active") : $this.removeClass("active"),

            $input.on("focusout", function () {
                $input.val().length ? $this.addClass("active") : $this.removeClass("active");
            }).on("focus", function () {
                $this.addClass("active");
            });
    });
});

function ProductDetailJs() {

    $(".product-gallery").slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        asNavFor: '.thumb-nav'
    });
    $(".thumb-nav").slick({
        slidesToShow: 4,
        slidesToScroll: 1,
        asNavFor: '.product-gallery',
        dots: false,
        focusOnSelect: true,
        prevArrow: '<div class="slick-prev"><i class="fal fa-angle-left"></i></div>',
        nextArrow: '<div class="slick-next"><i class="fal fa-angle-right"></i></div>'
    });
    $('[data-fancybox="gallery"]').fancybox({
        protect: true,
        loop: true,
        buttons: [
            'zoom',
            'slideShow',
            'close'
        ],
        animationEffect: "zoom-in-out",
        animationDuration: 366,
        transitionEffect: "tube",
        zoomOpacity: "auto"
    });

    $("#orderForm").on("submit", function (e) {
        e.preventDefault();
        $.post("/dat-hang-nhanh", $(this).serialize(), function (data) {
            if (data === "True") {
                $.toast({
                    heading: 'Cảm ơn bạn đã liên hệ. Chúng tôi sẽ liên hệ trong thời gian sớm nhất.',
                    icon: 'success'
                });
            } else {
                $.toast({
                    heading: 'Gửi thất bại',
                    text: 'Vui lòng điền đúng định dạng',
                    icon: 'error',
                })
            }
        });
    });
}