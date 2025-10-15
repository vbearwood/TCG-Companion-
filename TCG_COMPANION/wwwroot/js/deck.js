document.addEventListener("DOMContentLoaded", function () {
  const grid = document.getElementById("cards-grid");
  const form = document.getElementById("add-card-form");
  const strategyBtn = document.getElementById("generate-strategy");

  function loadDeck() {
    fetch("/api/Decks", { credentials: "same-origin" })
      .then((r) => {
        if (!r.ok) throw new Error(`HTTP error! status: ${r.status}`);
        return r.json();
      })
      .then((data) => {
        grid.innerHTML = "";
        const decks = Array.isArray(data) ? data : [data];
        
        if (!decks.length) {
          grid.innerHTML = "<p>No decks found.</p>";
          return;
        }

        const cards = decks[0]?.cards ?? decks[0]?.Cards ?? [];
        
        if (!cards.length) {
          grid.innerHTML = "<p>This deck has no cards yet.</p>";
          return;
        }

        cards.forEach((card) => {
          const img = document.createElement("img");
          img.src = card?.Image ?? card?.image ?? card?.images?.large ?? "";
          img.alt = card?.Name ?? card?.name ?? "Card";
          img.className = "img-fluid";
          grid.appendChild(img);
        });
      })
      .catch((err) => {
        grid.innerHTML = `<p>Could not load your deck. Error: ${err.message}</p>`;
      });
  }

  form?.addEventListener("submit", function (e) {
    e.preventDefault();
    fetch("/api/Decks", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Name: form.cardName.value,
        Set: form.setName.value,
        Number: form.cardNumber.value,
      }),
      credentials: "same-origin",
    }).then(() => {
      loadDeck();
      form.reset();
    });
  });

  strategyBtn?.addEventListener("click", function () {
    document.getElementById("strategy-box").style.display = "block";
    if (typeof window.AI === "function") window.AI();
  });

  loadDeck();
});