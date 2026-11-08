const API = 'http://localhost:5100/api';
const RAYON = 2; // vision 5x5

const carte = document.getElementById('carte');
const tuiles = new Map(); // "x,y" -> tuile connue
let jeton = localStorage.getItem('jeton');
let perso = null;

async function api(chemin, options = {}) {
	const reponse = await fetch(API + chemin, {
		...options,
		headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${jeton}` },
	});
	if (reponse.status === 401 && chemin !== '/auth/login') deconnecter();
	if (!reponse.ok) throw new Error(await reponse.text());
	return reponse.status === 204 ? null : reponse.json();
}

function memoriser(liste) {
	for (const t of liste) tuiles.set(`${t.positionX},${t.positionY}`, t);
}

function afficherInfos() {
	const champs = {
		Nom: perso.nom, Niveau: perso.niveau, Expérience: perso.experience,
		PV: `${perso.pointsVie} / ${perso.pointsVieMax}`, Force: perso.force,
		Défense: perso.defense, Position: `(${perso.positionX}, ${perso.positionY})`,
	};
	document.getElementById('infos').innerHTML =
		Object.entries(champs).map(([k, v]) => `<dt>${k}</dt><dd>${v}</dd>`).join('');
}

function selectionner(t) {
	document.getElementById('selection').innerHTML =
		`<dt>Position</dt><dd>(${t.positionX}, ${t.positionY})</dd>` +
		`<dt>Type</dt><dd>${t.type}</dd>` +
		`<dt>Traversable</dt><dd>${t.traversable ? 'Oui' : 'Non'}</dd>` +
		`<dt>Visites</dt><dd>${t.nbVisites}</dd>`;
}

function creerCase(x, y) {
	const div = document.createElement('div');
	div.className = 'tuile';
	if (x < 0 || y < 0 || x > 49 || y > 49) {
		div.classList.add('hors');
		return div;
	}

	const t = tuiles.get(`${x},${y}`);
	if (!t) {
		div.classList.add('vide');
		const bouton = document.createElement('button');
		bouton.textContent = '?';
		bouton.title = 'Explorer';
		bouton.onclick = async () => {
			try {
				memoriser([await api(`/Tuile/${x}/${y}`)]);
				dessinerCarte();
			} catch (e) { erreur(e.message); }
		};
		div.appendChild(bouton);
		return div;
	}

	const img = document.createElement('img');
	img.src = `assets/tiles/${t.imageUrl}`;
	img.alt = t.type;
	div.appendChild(img);
	div.onclick = () => selectionner(t);
	if (x === perso.positionX && y === perso.positionY) div.classList.add('joueur');
	return div;
}

function dessinerCarte() {
	carte.innerHTML = '';
	for (let y = perso.positionY - RAYON; y <= perso.positionY + RAYON; y++)
		for (let x = perso.positionX - RAYON; x <= perso.positionX + RAYON; x++)
			carte.appendChild(creerCase(x, y));
}

function erreur(message) {
	document.getElementById('erreur-jeu').textContent = message;
}

function majEtat({ personnage, tuiles: voisines }) {
	perso = personnage;
	memoriser(voisines);
	afficherInfos();
	dessinerCarte();
	erreur('');
}

async function deplacer(dx, dy) {
	try {
		const etat = await api('/personnage/deplacer', {
			method: 'PUT',
			body: JSON.stringify({ x: perso.positionX + dx, y: perso.positionY + dy }),
		});
		majEtat(etat);
	} catch (e) { erreur(e.message); }
}

async function demarrerJeu() {
	try {
		majEtat(await api('/personnage/moi'));
		document.getElementById('auth').hidden = true;
		document.getElementById('jeu').hidden = false;
		document.getElementById('deconnexion').hidden = false;
	} catch { /* jeton invalide : on reste sur l'écran de connexion */ }
}

function deconnecter() {
	jeton = null;
	localStorage.removeItem('jeton');
	tuiles.clear();
	document.getElementById('auth').hidden = false;
	document.getElementById('jeu').hidden = true;
	document.getElementById('deconnexion').hidden = true;
}

for (const [id, route] of [['form-connexion', 'login'], ['form-inscription', 'register']]) {
	document.getElementById(id).addEventListener('submit', async (e) => {
		e.preventDefault();
		try {
			const reponse = await api(`/auth/${route}`, {
				method: 'POST',
				body: JSON.stringify(Object.fromEntries(new FormData(e.target))),
			});
			jeton = reponse.jeton;
			localStorage.setItem('jeton', jeton);
			document.getElementById('erreur-auth').textContent = '';
			await demarrerJeu();
		} catch (err) {
			document.getElementById('erreur-auth').textContent = err.message;
		}
	});
}

document.getElementById('deconnexion').onclick = async () => {
	await api('/auth/logout', { method: 'POST' }).catch(() => {});
	deconnecter();
};

document.querySelectorAll('#fleches button').forEach((b) =>
	b.addEventListener('click', () => deplacer(+b.dataset.dx, +b.dataset.dy)));

const touches = { ArrowUp: [0, -1], ArrowDown: [0, 1], ArrowLeft: [-1, 0], ArrowRight: [1, 0] };
document.addEventListener('keydown', (e) => {
	if (perso && touches[e.key] && !document.getElementById('jeu').hidden) {
		e.preventDefault();
		deplacer(...touches[e.key]);
	}
});

if (jeton) demarrerJeu();
