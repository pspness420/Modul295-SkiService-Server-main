# Ski-Service Projekt (Modul 294)

Willkommen zum **Ski-Service Projekt**, einem umfassenden System, das ein Backend mit RESTful-API und ein responsives Frontend für den Ski-Service bietet. Dieses Projekt wurde im Rahmen des **Modul 294** entwickelt und umfasst eine vollständige Implementierung mit modernen Webtechnologien.

---

## Inhaltsverzeichnis

- [Ski-Service Projekt (Modul 294)](#ski-service-projekt-modul-294)
  - [Inhaltsverzeichnis](#inhaltsverzeichnis)
  - [Features](#features)
    - [Backend (Server):](#backend-server)
    - [Frontend:](#frontend)
  - [Voraussetzungen](#voraussetzungen)
  - [Installation und Setup](#installation-und-setup)
    - [Backend-Setup](#backend-setup)
    - [Frontend-Setup](#frontend-setup)
  - [Projektstruktur](#projektstruktur)
  - [API-Dokumentation](#api-dokumentation)
    - [Wichtige API-Endpunkte](#wichtige-api-endpunkte)
  - [Frontend-Inhalte](#frontend-inhalte)
    - [Seitenübersicht:](#seitenübersicht)
    - [Funktionen:](#funktionen)
  - [Technologien](#technologien)
    - [Backend:](#backend)
    - [Frontend:](#frontend-1)
  - [Autoren](#autoren)
  - [Hinweise](#hinweise)

---

## Features

### Backend (Server):

- RESTful-API zur Verwaltung der Anmeldungen.
- Swagger-Dokumentation der API unter `/api-docs`.
- Datenvalidierung und Duplikatsprüfung bei Anmeldungen.
- Option zum Löschen von Anmeldungen basierend auf Namen und E-Mail.
- Dynamisches Berechnen von Terminen basierend auf Priorität und Öffnungszeiten.

### Frontend:

- Benutzerfreundliche HTML-Seiten für verschiedene Aktionen:
  - Anmeldung für den Ski-Service.
  - Anzeige der Angebote.
  - Kontaktseite mit Google Maps Integration.
  - Datenschutz und Impressum.
- Responsive Design für alle Bildschirmgrößen.
- Ladeanimation (Loader) beim Start.

---

## Voraussetzungen

- **Node.js** (Version >= 14)
- **npm** (Node Package Manager)
- Webbrowser (z. B. Chrome, Firefox)

---

## Installation und Setup

### Backend-Setup

1. **Projekt initialisieren**:

   ```bash
   npm init --y
   ```

2. **Notwendige Abhängigkeiten installieren**:

   ```bash
   npm install --save express dotenv cors swagger-ui-express
   ```

3. **Entwicklungsabhängigkeiten installieren**:

   ```bash
   npm install --save-dev nodemon
   ```

4. **Projekt starten**:

   - **Produktionsmodus**:
     ```bash
     npm start
     ```
   - **Entwicklungsmodus**:
     ```bash
     npm run dev
     ```

5. **Wichtig:**

   - Führen Sie das Projekt **nicht in einem OneDrive-Ordner** aus, da dies zu Problemen führen kann.

### Frontend-Setup

1. Kopieren Sie die HTML-, CSS- und JavaScript-Dateien in einen Ordner, der vom Webserver bereitgestellt wird.
2. Stellen Sie sicher, dass die Links zu CSS und JavaScript-Dateien korrekt sind.
3. Öffnen Sie die `index.html` in einem Webbrowser.


## Projektstruktur

Hier ist eine Übersicht über die Projektstruktur:
```
Modul294-SkiService-Server/
├── css/               # CSS-Dateien für das Frontend
├── html/              # HTML-Seiten
├── js/                # JavaScript-Dateien für das Frontend
├── server/            # Backend-Server und API-Logik
│   ├── controllers/   # Controller-Logik für die API
│   ├── data/          # Daten (z. B. JSON-Dateien)
│   ├── models/        # Datenmodelle
│   ├── routes/        # API-Routen
│   └── server.js      # Einstiegspunkt für den Server
├── .env               # Umgebungsvariablen
├── package.json       # Projektkonfiguration und Abhängigkeiten
├── README.md          # Dokumentation des Projekts
└── swagger.json       # Swagger-Dokumentation für die API
```

---

## API-Dokumentation

Nach dem Start des Servers können Sie die API-Dokumentation unter folgendem Link aufrufen:

```
http://localhost:5000/api-docs
```

### Wichtige API-Endpunkte

- **GET** `/api/registrations` - Alle Anmeldungen abrufen
- **POST** `/api/registration` - Neue Anmeldung hinzufügen
- **DELETE** `/api/registration/:id` - Anmeldung löschen

---

## Frontend-Inhalte

### Seitenübersicht:

1. **Home (`index.html`)**:
   - Begrüßungsseite mit Bannern und Informationen zum Service.
2. **Angebot (`services.html`)**:
   - Detaillierte Beschreibung der angebotenen Dienstleistungen.
3. **Kontakt (`contact.html`)**:
   - Kontaktinformationen und Google Maps Integration.
4. **Anmeldung (`registration.html`)**:
   - Formular zur Registrierung für den Ski-Service.
5. **Datenschutz (`datenschutz.html`)** und **Impressum (`impressum.html`)**:
   - Rechtliche Informationen.

### Funktionen:

- Formularüberprüfung (z. B. E-Mail-Format und Duplikatprüfung).
- Dynamische Buttons:
  - Anmeldung absenden.
  - Bestehende Anmeldungen ansehen.
  - Anmeldung löschen (nach Bestätigung).
- Responsives Layout mit Bootstrap und CSS Media Queries.

---

## Technologien

### Backend:

- **Node.js** und **Express** für den Server.
- **dotenv** zur Verwaltung von Umgebungsvariablen.
- **cors** für Cross-Origin Resource Sharing.
- **Swagger** zur API-Dokumentation.

### Frontend:

- **HTML**, **CSS**, **Bootstrap** für Design und Layout.
- **JavaScript** für Interaktivität.
- **Google Maps API** für Standortanzeige.

---

## Autoren

- **Projektleitung:** Yannick Frei
- **Entwicklung:** Yannick Frei, Tunahan Keser, Felipe Oliveira de Carvalho

---

## Hinweise

- Testen Sie das Projekt in einer lokalen Umgebung mit Node.js und einem modernen Browser.
- Stellen Sie sicher, dass alle Verbindungen korrekt eingerichtet sind, insbesondere bei API-Calls.

---


**Ski-Service Projekt** 😊