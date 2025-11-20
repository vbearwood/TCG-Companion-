function AI() {
    const form = document.getElementById("strategy-form");
    const box = document.getElementById("strategy-box");
    const text = document.getElementById("strategy-text");

    if (!form) return;

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        box.style.display = "block";
        text.textContent = "Generating strategy…";

        try {
            const formData = new FormData(form);

            const resp = await fetch(form.action, {
                method: "POST",
                body: formData
            });

            const html = await resp.text();
            text.textContent = html || "No strategy returned.";
        } catch (err) {
            text.textContent = "Error: " + err.message;
        }
    });
}

document.addEventListener("DOMContentLoaded", AI)