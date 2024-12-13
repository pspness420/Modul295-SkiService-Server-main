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

function refreshToken() {
    const refreshToken = localStorage.getItem("refreshToken");
    fetch("http://localhost:5000/api/auth/refresh", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ accessToken, refreshToken })
    })
        .then(response => response.json())
        .then(data => {
            if (data.accessToken) {
                localStorage.setItem("accessToken", data.accessToken);
                localStorage.setItem("refreshToken", data.refreshToken);
            } else {
                console.error("Token-Aktualisierung fehlgeschlagen.");
            }
        })
        .catch(error => console.error("Fehler bei der Token-Aktualisierung:", error));
}

function logout() {
    const refreshToken = localStorage.getItem("refreshToken");
    fetch("http://localhost:5000/api/auth/logout", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(refreshToken)
    })
        .then(() => {
            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
            alert("Erfolgreich abgemeldet!");
        })
        .catch(error => console.error("Logout-Fehler:", error));
}

document.getElementById("registerForm")?.addEventListener("submit", async (e) => {
    e.preventDefault();

    const firstName = document.getElementById("firstName").value.trim();
    const lastName = document.getElementById("lastName").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();
    const confirmPassword = document.getElementById("confirmPassword").value.trim();

    if (password !== confirmPassword) {
        alert("Passwörter stimmen nicht überein.");
        return;
    }

    const username = `${firstName} ${lastName}`;

    try {
        const response = await fetch("http://localhost:5000/api/auth/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Benutzername: username, Passwort: password, Email: email }),
        });

        if (!response.ok) throw new Error("Registrierung fehlgeschlagen.");

        alert("Registrierung erfolgreich!");
        location.href = "login.html";
    } catch (error) {
        alert("Fehler bei der Registrierung: " + error.message);
    }
});
