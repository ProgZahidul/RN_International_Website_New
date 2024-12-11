// JavaScript to rotate background images
const heroSection = document.querySelector('.hero-section');

// Array of image URLs
const images = [
    '/images/image1.jpg',
    '/images/image2.jpg',
    '/images/image3.jpg',
    '/images/image4.jpg'
];

// Index to track the current image
let currentIndex = 0;

// Function to change the background image
function changeBackgroundImage() {
    heroSection.style.backgroundImage = `url(${images[currentIndex]})`;
    currentIndex = (currentIndex + 1) % images.length; // Loop through images
}

// Initial image setup
changeBackgroundImage();

// Change image every 2 seconds
setInterval(changeBackgroundImage, 4000);

// Close the navbar when clicking outside of it
document.addEventListener('click', function (event) {
    const navbarMenu = document.getElementById('navbarNav');
    const toggler = document.querySelector('.navbar-toggler');

    if (!navbarMenu.contains(event.target) && !toggler.contains(event.target)) {
        // Check if the menu is currently shown and close it
        if (navbarMenu.classList.contains('show')) {
            bootstrap.Collapse.getInstance(navbarMenu).hide();
        }
    }
});

// Close the navbar when a nav link is clicked
document.querySelectorAll('.navbar-nav .nav-link').forEach(function (link) {
    link.addEventListener('click', function () {
        bootstrap.Collapse.getInstance(document.getElementById('navbarNav')).hide();
    });
});


