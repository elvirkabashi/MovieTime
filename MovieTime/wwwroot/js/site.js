const shrink_btn = document.querySelector(".shrink-btn");
const search = document.querySelector(".search");
const sidebar_links = document.querySelectorAll(".sidebar-links a");
const active_tab = document.querySelector(".active-tab");
const shortcuts = document.querySelector(".sidebar-links h4");
const tooltip_elements = document.querySelectorAll(".tooltip-element");

let activeIndex;

// Function to toggle the shrink class on body
shrink_btn.addEventListener("click", () => {
    document.body.classList.toggle("shrink");
    setTimeout(moveActiveTab, 400);

    shrink_btn.classList.add("hovered");

    setTimeout(() => {
        shrink_btn.classList.remove("hovered");
    }, 500);
});

// Function to handle the search click
search.addEventListener("click", () => {
    document.body.classList.remove("shrink");
    search.lastElementChild.focus();
});

// Function to move the active tab based on the clicked link
function moveActiveTab() {
    let topPosition = activeIndex * 58 + 2.5;

    if (activeIndex > 3) {
        topPosition += shortcuts.clientHeight;
    }

    active_tab.style.top = `${topPosition}px`;
}

// Function to change the active link and move the active tab
function changeLink() {
    sidebar_links.forEach((sideLink) => sideLink.classList.remove("active"));
    this.classList.add("active");

    activeIndex = this.dataset.active;

    moveActiveTab();
}

// Add click event listener to each sidebar link
sidebar_links.forEach((link) => link.addEventListener("click", changeLink));

// Function to show tooltip when hovering over an element
function showTooltip() {
    let tooltip = this.parentNode.lastElementChild;
    let spans = tooltip.children;
    let tooltipIndex = Array.from(tooltip_elements).indexOf(this);

    Array.from(spans).forEach((sp) => sp.classList.remove("show"));
    spans[tooltipIndex].classList.add("show");

    tooltip.style.top = `${(100 / (spans.length * 2)) * (tooltipIndex * 2 + 1)}%`;
}

// Add mouseover event listener to each tooltip element
tooltip_elements.forEach((elem) => {
    elem.addEventListener("mouseover", showTooltip);
});

// Initialization: Set the active link based on the current page URL
document.addEventListener("DOMContentLoaded", () => {
    const currentPath = window.location.pathname;
    sidebar_links.forEach((link, index) => {
        if (link.getAttribute("href") === currentPath) {
            activeIndex = index;
            link.classList.add("active");
            moveActiveTab();
        }
    });
});
