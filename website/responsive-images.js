window.onload = function() {
	const images = document.querySelectorAll("img");

	images.forEach(image => {
	image.classList.add("img-fluid");
	});
}