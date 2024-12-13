// =========================================================================================
// Event-Listener für das Formular (Submit-Event)
// =========================================================================================
document.getElementById("registrationForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars (Seitenreload)

    // Holen der Eingabewerte aus den Formularfeldern
    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const kundenName = `${firstName} ${lastName}`; // Backend erwartet KundenName
    const email = document.getElementById("email").value.trim();
    const telefon = document.getElementById("phone").value.trim();
    const prioritaet = document.getElementById("priority").value;

    // Holen des Hauptservices (Radio-Buttons)
    const serviceType = document.querySelector('input[name="serviceType"]:checked');
    if (!serviceType) {
        alert("Bitte wählen Sie einen Hauptservice aus (Kleiner oder Großer Service).");
        return;
    }

    // Holen der zusätzlichen Dienstleistungen (Checkboxen)
    const additionalServices = Array.from(document.querySelectorAll('input[type="checkbox"]:checked'))
        .map((checkbox) => checkbox.value);

    // Validierung der Eingabefelder
    if (!validateEmail(email)) {
        alert("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
        return;
    }
    if (!validatePhone(telefon)) {
        alert("Bitte geben Sie die Telefonnummer im Format 081 581 92 35 ein.");
        return;
    }

    // Zusammenfügen der Dienstleistungen (Hauptservice + zusätzliche Services)
    const dienstleistung = [serviceType.value, ...additionalServices].join(", "); // Backend erwartet Dienstleistung

    // Zusammenstellen der Anmeldedaten
    const registrationData = {
        kundenName: kundenName, // Backend erwartet KundenName
        email: email,
        telefon: telefon, // Backend erwartet Telefon
        prioritaet: prioritaet, // Backend erwartet Prioritaet
        dienstleistung: dienstleistung, // Backend erwartet Dienstleistung
        createDate: new Date().toISOString(), // Backend erwartet CreateDate
        pickupDate: calculatePickupDate(prioritaet), // Backend erwartet PickupDate
        status: "Offen", // Backend erwartet Status
    };

    // Überprüfung auf Duplikate
    checkDuplicateRegistration(registrationData)
        .then((isDuplicate) => {
            if (isDuplicate) {
                alert("Eine Anmeldung mit dieser E-Mail-Adresse oder diesem Namen existiert bereits.");
            } else {
                // Daten an die API senden
                sendRegistrationData(registrationData);

                // Formular zurücksetzen
                document.getElementById("registrationForm").reset();

                // Aktualisiere die Standardanzeige für die Priorität
                updatePriorityInfo("Tief");

                // Erfolgsmeldung anzeigen
                alert("Anmeldung erfolgreich gespeichert!");
            }
        })
        .catch((error) => {
            alert("Fehler bei der Überprüfung der Anmeldung.");
            console.error("Fehler:", error);
        });
});

// =========================================================================================
// Hilfsfunktionen
// =========================================================================================

// Funktion zur Überprüfung auf Duplikate
function checkDuplicateRegistration(registrationData) {
    return fetch("http://localhost:5000/api/auftrag")
        .then((response) => {
            if (!response.ok) {
                throw new Error("Fehler beim Abrufen der bestehenden Anmeldungen.");
            }
            return response.json();
        })
        .then((data) => {
            return data.some(
                (existingRegistration) =>
                    existingRegistration.email === registrationData.email ||
                    (existingRegistration.kundenName === registrationData.kundenName &&
                        existingRegistration.email === registrationData.email)
            );
        });
}

// Funktion zur Berechnung des Abholdatums basierend auf der Priorität und Öffnungszeiten
function calculatePickupDate(prioritaet) {
    let daysToAdd;
    if (prioritaet === "Tief") daysToAdd = 12;
    else if (prioritaet === "Standard") daysToAdd = 7;
    else if (prioritaet === "Express") daysToAdd = 5;
    else daysToAdd = 7;

    let pickupDate = new Date();
    pickupDate.setDate(pickupDate.getDate() + daysToAdd);

    while (isNonWorkingDay(pickupDate)) {
        pickupDate.setDate(pickupDate.getDate() + 1);
    }

    return pickupDate.toISOString();
}

// Funktion zur Überprüfung, ob ein Tag ein arbeitsfreier Tag ist (Sonntag)
function isNonWorkingDay(date) {
    const day = date.getDay();
    return day === 0;
}

// Funktion zur Validierung der E-Mail-Adresse
function validateEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Funktion zur Validierung der Telefonnummer
function validatePhone(telefon) {
    const phoneRegex = /^\d{3} \d{3} \d{2} \d{2}$/;
    return phoneRegex.test(telefon);
}

// Funktion zum Senden der Anmeldedaten an die API
function sendRegistrationData(data) {
    fetch("http://localhost:5000/api/auftrag", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error("Fehler beim Speichern der Daten.");
            }
            return response.json();
        })
        .then((responseData) => {
            console.log("Server-Antwort:", responseData);
        })
        .catch((error) => {
            alert("Beim Speichern ist ein Fehler aufgetreten.");
            console.error("Fehler:", error);
        });
}

// =========================================================================================
// Event-Listener für Änderungen der Priorität
// =========================================================================================
document.getElementById("priority").addEventListener("change", function () {
    updatePriorityInfo(this.value);
});

// Funktion zum Aktualisieren der Anzeige basierend auf der Priorität
function updatePriorityInfo(prioritaet) {
    const priorityData = {
        Tief: { daysToAdd: 12, description: "Zusätzliche Tage: +5, Total: 12 Tage bis zur Fertigstellung" },
        Standard: { daysToAdd: 7, description: "Zusätzliche Tage: 0, Total: 7 Tage bis zur Fertigstellung" },
        Express: { daysToAdd: 5, description: "Zusätzliche Tage: -2, Total: 5 Tage bis zur Fertigstellung" },
    };

    const infoElement = document.getElementById("priorityInfo");

    const today = new Date();
    const daysToAdd = priorityData[prioritaet]?.daysToAdd || 0;
    let pickupDate = new Date(today);
    pickupDate.setDate(today.getDate() + daysToAdd);

    while (isNonWorkingDay(pickupDate)) {
        pickupDate.setDate(pickupDate.getDate() + 1);
    }

    const formattedDate = pickupDate.toLocaleDateString("de-DE");

    if (priorityData[prioritaet]) {
        infoElement.textContent = `${priorityData[prioritaet].description} | Fertigstellungsdatum: ${formattedDate}`;
    } else {
        infoElement.textContent = "";
    }
}

// Initiale Anzeige für "Tief" (default)
updatePriorityInfo("Tief");

// =========================================================================================
// Event-Listener für die Telefonnummer-Eingabe
// =========================================================================================
document.getElementById("phone").addEventListener("input", function (e) {
    const input = e.target; // Aktuelles Eingabefeld
    let value = input.value.replace(/\D/g, ''); // Entfernt alle Nicht-Zahlen
    const maxLength = 10; // Maximale Anzahl von Ziffern
    const cursorPosition = input.selectionStart; // Aktuelle Cursorposition
    const prevLength = input.dataset.prevLength || 0; // Vorherige Länge der Eingabe
    const isDeleting = value.length < prevLength; // Überprüfung, ob der Benutzer löscht

    // Beschränke die Eingabe auf maximal 10 Ziffern
    value = value.slice(0, maxLength);

    // Formatiere die Eingabe in Blöcke von 3 3 2 2
    let formattedValue = value
        .replace(/(\d{3})(\d{0,3})/, '$1 $2') // Fügt den ersten Abstand ein
        .replace(/(\d{3}) (\d{3})(\d{0,2})/, '$1 $2 $3') // Fügt den zweiten Abstand ein
        .replace(/(\d{3}) (\d{3}) (\d{2})(\d{0,2})/, '$1 $2 $3 $4'); // Fügt den dritten Abstand ein

    // Setze den formatierten Wert in das Eingabefeld
    input.value = formattedValue;

    // Korrigiere die Cursorposition, wenn Leerzeichen beim Löschen entstehen
    if (isDeleting && cursorPosition > 0 && formattedValue[cursorPosition - 1] === ' ') {
        input.setSelectionRange(cursorPosition - 1, cursorPosition - 1);
    } else if (!isDeleting) {
        input.setSelectionRange(cursorPosition + (formattedValue.length - prevLength), cursorPosition + (formattedValue.length - prevLength));
    }

    // Speichere die aktuelle Länge der Eingabe
    input.dataset.prevLength = formattedValue.length;
});
