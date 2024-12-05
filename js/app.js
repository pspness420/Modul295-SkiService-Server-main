// =========================================================================================
// Event-Listener für das Formular (Submit-Event)
// =========================================================================================
document.getElementById("registrationForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Verhindert das Standardverhalten des Formulars (Seitenreload)

    // Holen der Eingabewerte aus den Formularfeldern
    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const name = `${firstName} ${lastName}`;
    const email = document.getElementById("email").value.trim();
    const phone = document.getElementById("phone").value.trim();
    const priority = document.getElementById("priority").value;

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
    if (!validatePhone(phone)) {
        alert("Bitte geben Sie die Telefonnummer im Format 081 581 92 35 ein.");
        return;
    }

    // Zusammenfügen der Dienstleistungen (Hauptservice + zusätzliche Services)
    const allServices = [serviceType.value, ...additionalServices].join(", ");

    // Zusammenstellen der Anmeldedaten
    const registrationData = {
        name: name,
        email: email,
        phone: phone,
        priority: priority,
        service: allServices,
        create_date: new Date().toISOString(),
        pickup_date: calculatePickupDate(priority),
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
// Event-Listener für den "Daten löschen"-Button
// =========================================================================================
document.getElementById("resetButton").addEventListener("click", function () {
    const firstName = prompt("Bitte geben Sie den Vornamen der Anmeldung ein:");
    if (!firstName) {
        alert("Der Vorname ist erforderlich, um fortzufahren.");
        return;
    }

    const lastName = prompt("Bitte geben Sie den Nachnamen der Anmeldung ein:");
    if (!lastName) {
        alert("Der Nachname ist erforderlich, um fortzufahren.");
        return;
    }

    const email = prompt("Bitte geben Sie die E-Mail-Adresse der Anmeldung ein:");
    if (!email) {
        alert("Die E-Mail-Adresse ist erforderlich, um fortzufahren.");
        return;
    }

    const name = `${firstName.trim()} ${lastName.trim()}`;

    // Daten vom Server abrufen und überprüfen
    fetch("http://localhost:5000/api/registrations")
        .then((response) => {
            if (!response.ok) {
                alert("Fehler beim Abrufen der Daten vom Server.");
                throw new Error("Fehler beim Abrufen der Daten.");
            }
            return response.json();
        })
        .then((data) => {
            const matchingRegistration = data.find(
                (registration) => registration.name === name && registration.email === email
            );

            if (matchingRegistration) {
                deleteRegistration(matchingRegistration.id);
            } else {
                alert("Keine passende Anmeldung gefunden.");
            }
        })
        .catch((error) => {
            alert("Beim Abrufen der Daten ist ein Fehler aufgetreten.");
            console.error("Fehler:", error);
        });
});

// Funktion zum Löschen der Anmeldung
function deleteRegistration(id) {
    fetch(`http://localhost:5000/api/registration/${id}`, {
        method: "DELETE",
    })
        .then((response) => {
            if (!response.ok) {
                alert("Fehler beim Löschen der Anmeldung.");
                throw new Error("Fehler beim Löschen der Anmeldung.");
            }
            alert("Die Anmeldung wurde erfolgreich gelöscht.");
        })
        .catch((error) => {
            alert("Beim Löschen der Anmeldung ist ein Fehler aufgetreten.");
            console.error("Fehler:", error);
        });
}

// =========================================================================================
// Hilfsfunktionen
// =========================================================================================

// Funktion zur Überprüfung auf Duplikate
function checkDuplicateRegistration(registrationData) {
    return fetch("http://localhost:5000/api/registrations")
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
                    (existingRegistration.name === registrationData.name &&
                        existingRegistration.email === registrationData.email)
            );
        });
}

// Funktion zur Berechnung des Abholdatums basierend auf der Priorität und Öffnungszeiten
function calculatePickupDate(priority) {
    let daysToAdd;
    if (priority === "Tief") daysToAdd = 12;
    else if (priority === "Standard") daysToAdd = 7;
    else if (priority === "Express") daysToAdd = 5;
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
function validatePhone(phone) {
    const phoneRegex = /^\d{3} \d{3} \d{2} \d{2}$/;
    return phoneRegex.test(phone);
}

// Funktion zum Senden der Anmeldedaten an die API
function sendRegistrationData(data) {
    fetch("http://localhost:5000/api/registration", {
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
function updatePriorityInfo(priority) {
    const priorityData = {
        Tief: { daysToAdd: 12, description: "Zusätzliche Tage: +5, Total: 12 Tage bis zur Fertigstellung" },
        Standard: { daysToAdd: 7, description: "Zusätzliche Tage: 0, Total: 7 Tage bis zur Fertigstellung" },
        Express: { daysToAdd: 5, description: "Zusätzliche Tage: -2, Total: 5 Tage bis zur Fertigstellung" },
    };

    const infoElement = document.getElementById("priorityInfo");

    const today = new Date();
    const daysToAdd = priorityData[priority]?.daysToAdd || 0;
    let pickupDate = new Date(today);
    pickupDate.setDate(today.getDate() + daysToAdd);

    while (isNonWorkingDay(pickupDate)) {
        pickupDate.setDate(pickupDate.getDate() + 1);
    }

    const formattedDate = pickupDate.toLocaleDateString("de-DE");

    if (priorityData[priority]) {
        infoElement.textContent = `${priorityData[priority].description} | Fertigstellungsdatum: ${formattedDate}`;
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