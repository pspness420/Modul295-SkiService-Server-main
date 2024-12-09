document.getElementById("loginForm")?.addEventListener("submit", async (e) => {
    e.preventDefault();

    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();

    try {
        const response = await fetch("http://localhost:5000/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Benutzername: username, Passwort: password }),
        });

        if (!response.ok) throw new Error("Login fehlgeschlagen.");

        const data = await response.json();
        localStorage.setItem("jwtToken", data.Token);

        alert("Login erfolgreich!");
        location.href = "index.html";
    } catch (error) {
        alert("Fehler beim Login: " + error.message);
    }
});

document.getElementById("registerForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();
    const confirmPassword = document.getElementById("confirmPassword").value.trim();

    if (password !== confirmPassword) {
        alert("Passwörter stimmen nicht überein!");
        return;
    }

    const userData = {
        vorname: firstName,
        nachname: lastName,
        email: email,
        passwort: password,
    };

    try {
        const response = await fetch("http://localhost:5000/api/auth/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(userData),
        });

        if (response.ok) {
            alert("Registrierung erfolgreich!");
        } else {
            alert("Registrierung fehlgeschlagen.");
        }
    } catch (error) {
        console.error("Fehler:", error);
        alert("Ein Fehler ist aufgetreten.");
    }
});

