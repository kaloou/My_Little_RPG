const API_BASE = "http://localhost:5100/api/Tuile";
const CENTRE_X = 10;
const CENTRE_Y = 10;
const RAYON = 2;

const carte = document.getElementById("carte");

async function chargerTuile(x, y) {
    const reponse = await fetch(`${API_BASE}/${x}/${y}`);
    return reponse.json();
}

function afficherTuile(div, tuile) {
    div.innerHTML = "";
    div.classList.remove("vide");

    const img = document.createElement("img");
    img.src = `../assets/tiles/${tuile.imageUrl}`;
    img.alt = tuile.type;
    div.appendChild(img);
}

function creerCaseVide(x, y) {
    const div = document.createElement("div");
    div.className = "tuile vide";
    div.dataset.x = x;
    div.dataset.y = y;

    const bouton = document.createElement("button");
    bouton.textContent = "?";
    bouton.addEventListener("click", async () => {
        const tuile = await chargerTuile(x, y);
        afficherTuile(div, tuile);
    });
    div.appendChild(bouton);

    return div;
}

async function initCarte() {
    carte.innerHTML = "";

    for (let y = CENTRE_Y - RAYON; y <= CENTRE_Y + RAYON; y++) {
        for (let x = CENTRE_X - RAYON; x <= CENTRE_X + RAYON; x++) {
            const div = creerCaseVide(x, y);
            carte.appendChild(div);
        }
    }

    const centre = carte.querySelector(`[data-x="${CENTRE_X}"][data-y="${CENTRE_Y}"]`);
    const tuileCentrale = await chargerTuile(CENTRE_X, CENTRE_Y);
    afficherTuile(centre, tuileCentrale);
}

initCarte();
