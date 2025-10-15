function AI () {
  const form = document.getElementById("strategy-form");
  const box  = document.getElementById("strategy-box");
  const text = document.getElementById("strategy-text");

  if (!form) return;

  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const btn = form.querySelector('button[type="submit"]');
    if (btn) { btn.disabled = true; btn.textContent = "Generating…"; }

    if (box)  box.style.display = "block";
    if (text) text.textContent  = "Analyzing deck…";

    try {
      const deckRes = await fetch("/api/Decks", { credentials: "include" });
      if (!deckRes.ok) throw new Error("Unable to load deck.");
      const decks = await deckRes.json();
      const deck  = (decks && decks[0]) ? decks[0] : null;

      const fallbackFromDom = () => {
        const imgs = Array.from(document.getElementsByClassName("Gallery"));
        return {
          cards: imgs.map(img => ({
            name: img.alt || "Card",
            imageUrl: img.src || ""
          }))
        };
      };

      const deckObj = deck && deck.cards && deck.cards.length ? deck : fallbackFromDom();

      const payload = { Deck: JSON.stringify(deckObj) };

      const resp = await fetch("/api/Strategy/deck", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(payload)
      });

    
      const raw = await resp.text();
      if (!resp.ok) throw new Error(raw || "Strategy request failed.");

      
      let strategyText = raw;
      if ((raw.startsWith('"') && raw.endsWith('"')) || (raw.startsWith("'") && raw.endsWith("'"))) {
        try { strategyText = JSON.parse(raw); } catch { /* keep raw */ }
      }

      if (text) text.textContent = strategyText || "No strategy returned.";
    } catch (err) {
      if (text) text.textContent = err?.message || "Request failed.";
    } finally {
      if (btn) { btn.disabled = false; btn.textContent = "Generate Deck Strategy"; }
    }
  });
}
document.addEventListener("DOMContentLoaded", () => {
  if (typeof AI === "function") AI();
});