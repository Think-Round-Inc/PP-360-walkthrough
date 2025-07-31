//js to add functionality to the add to cart 

        document.addEventListener("DOMContentLoaded", function() {
            const addToCartButtons = document.querySelectorAll('.add-to-cart');
            const cartCountBadge = document.querySelector('.cart-count');

            addToCartButtons.forEach(button => {
                button.addEventListener('click', function() {
                    const productId = button.getAttribute('data-product-id');
                    addToCart(productId);
                });
            });

            function addToCart(productId) {
                // Here you can add the logic to add the item to the cart
                // For simplicity, let's just update the cart count
                updateCartCount();
            }

            function updateCartCount() {
                // Here you can fetch the current cart count from the backend
                // For simplicity, let's just increment the count by 1
                let currentCount = parseInt(cartCountBadge.textContent);
                cartCountBadge.textContent = currentCount + 1;
            }
        });

        class StickyNavigation {
	
            constructor() {
                this.currentId = null;
                this.currentTab = null;
                this.tabContainerHeight = 70;
                let self = this;
                $('.et-hero-tab').click(function() { 
                    self.onTabClick(event, $(this)); 
                });
                $(window).scroll(() => { this.onScroll(); });
                $(window).resize(() => { this.onResize(); });
            }
            
            onTabClick(event, element) {
                event.preventDefault();
                let scrollTop = $(element.attr('href')).offset().top - this.tabContainerHeight + 1;
                $('html, body').animate({ scrollTop: scrollTop }, 600);
            }
            
            onScroll() {
                this.checkTabContainerPosition();
            this.findCurrentTabSelector();
            }
            
            onResize() {
                if(this.currentId) {
                    this.setSliderCss();
                }
            }
            
            checkTabContainerPosition() {
                let offset = $('.et-hero-tabs').offset().top + $('.et-hero-tabs').height() - this.tabContainerHeight;
                if($(window).scrollTop() > offset) {
                    $('.et-hero-tabs-container').addClass('et-hero-tabs-container--top');
                } 
                else {
                    $('.et-hero-tabs-container').removeClass('et-hero-tabs-container--top');
                }
            }
            
            findCurrentTabSelector(element) {
                let newCurrentId;
                let newCurrentTab;
                let self = this;
                $('.et-hero-tab').each(function() {
                    let id = $(this).attr('href');
                    let offsetTop = $(id).offset().top - self.tabContainerHeight;
                    let offsetBottom = $(id).offset().top + $(id).height() - self.tabContainerHeight;
                    if($(window).scrollTop() > offsetTop && $(window).scrollTop() < offsetBottom) {
                        newCurrentId = id;
                        newCurrentTab = $(this);
                    }
                });
                if(this.currentId != newCurrentId || this.currentId === null) {
                    this.currentId = newCurrentId;
                    this.currentTab = newCurrentTab;
                    this.setSliderCss();
                }
            }
            
            setSliderCss() {
                let width = 0;
                let left = 0;
                if(this.currentTab) {
                    width = this.currentTab.css('width');
                    left = this.currentTab.offset().left;
                }
                $('.et-hero-tab-slider').css('width', width);
                $('.et-hero-tab-slider').css('left', left);
            }
            
        }
        
        new StickyNavigation();
