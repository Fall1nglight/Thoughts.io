# Thoughts.io [EN]

A full-stack web application where users can securely share their thoughts with the world or keep them private.

---

## 🎯 Project Goal

The goal of **Thoughts.io** is to create a clean and user-friendly platform where registered users can create short text entries, or "thoughts". The system allows for controlling the visibility of these entries (public or private), providing a space for both personal notes and community interactions. The project showcases modern web development techniques through a well-structured, maintainable codebase, strictly separating front-end and back-end logic.

---

## ✨ Key Features

### For All Visitors (Unauthenticated)

- **Browse Public Thoughts:** The main page lists the latest public entries.
- **View Reactions:** Visitors can see how others have reacted to public thoughts.
- **Registration & Login:** Ability to create a new account and log into an existing one.

### For Logged-in Users

- **Thought Management (CRUD):** Create, edit, and delete your own thoughts on a dedicated user dashboard.
- **Set Visibility:** Each thought's visibility can be individually set to private (only the author can see it) or public (everyone can see it).
- **Interaction:** Add reactions (e.g., likes) to other users' public thoughts.
- **Personal Dashboard:** Users can view all their created thoughts, including private ones.

### Admin Panel

- **User Management:** Administrators can view and manage registered users' data.
- **Content Moderation:** Ability to edit or delete any thought from any user, ensuring the platform's integrity.
- **Statistics:** An overview dashboard to monitor key metrics (e.g., number of registered users, number of thoughts created).

---

## 🛠️ Technologies Used

The project is built on a modern, service-oriented architecture.

### Front-end

- **Framework:** [Vue.js](https://vuejs.org/)
- **State Management:** [Pinia](https://pinia.vuejs.org/)
- **Routing:** [Vue Router](https://router.vuejs.org/)
- **UI:** [Bootstrap](https://getbootstrap.com/)
- **HTTP Client:** [Axios](https://axios-http.com/)
- **Validation:** [Joi](https://joi.dev/)
- **Date Handling:** [date-fns](https://date-fns.org/)

### Back-end

- **Framework:** [C# .NET](https://dotnet.microsoft.com/)
- **Data Access:** [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- **Validation:** [FluentValidation](https://fluentvalidation.net/)
- **API Documentation:** [Swashbuckle (Swagger)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- **Logging:** [Serilog](https://serilog.net/)
- **Caching:** [FusionCache](https://github.com/jodydonetti/FusionCache)
- **Password Hashing:** [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net)

### Database

- **System:** Microsoft SQL Server (Express)

---

# Thoughts.io [HU]

Egy full-stack webalkalmazás, ahol a felhasználók biztonságosan megoszthatják gondolataikat a világgal vagy megtarthatják azokat privátan.

---

## 🎯 Projekt Célja

A **Thoughts.io** célja egy letisztult és felhasználóbarát platform létrehozása, ahol a regisztrált felhasználók rövid szöveges bejegyzéseket, "gondolatokat" hozhatnak létre. A rendszer lehetővé teszi a bejegyzések láthatóságának szabályozását (publikus vagy privát), ezzel teret adva a személyes jegyzeteknek és a közösségi interakcióknak egyaránt. A projekt bemutatja a modern webfejlesztési technikákat egy jól strukturált, karbantartható kódbázison keresztül, a front-end és back-end logikát szigorúan elválasztva.

---

## ✨ Főbb Funkciók

### Bárki számára (bejelentkezés nélkül)

- **Publikus gondolatok böngészése:** A főoldalon listázva láthatók a legfrissebb publikus bejegyzések.
- **Reakciók megtekintése:** A látogatók láthatják, hogy mások hogyan reagáltak egy-egy publikus gondolatra.
- **Regisztráció és bejelentkezés:** Lehetőség új fiók létrehozására és a meglévőbe való belépésre.

### Bejelentkezett felhasználóknak

- **Gondolatok kezelése (CRUD):** Saját gondolatok létrehozása, szerkesztése és törlése egy dedikált felhasználói felületen.
- **Láthatóság beállítása:** Minden gondolatnál egyedileg állítható, hogy az privát (csak a szerző láthatja) vagy publikus (bárki láthatja) legyen.
- **Interakció:** Reakciók (pl. kedvelés) hozzáadása mások publikus gondolataihoz.
- **Saját profil:** A felhasználó megtekintheti a saját maga által létrehozott összes gondolatot, a privátakat is beleértve.

### Adminisztrátori felület

- **Felhasználókezelés:** Az adminisztrátorok képesek a regisztrált felhasználók adatainak megtekintésére és kezelésére.
- **Tartalommoderálás:** Bármely felhasználó bármely gondolatát képes módosítani vagy törölni, biztosítva ezzel a platform tisztaságát.
- **Statisztikák:** Egy áttekintő műszerfal, ahol a legfontosabb metrikák (pl. regisztrált felhasználók száma, létrehozott gondolatok száma) követhetők.

---

## 🛠️ Felhasznált Technológiák

A projekt egy modern, szervizorientált architektúrára épül.

### Front-end

- **Framework:** [Vue.js](https://vuejs.org/)
- **State Management:** [Pinia](https://pinia.vuejs.org/)
- **Routing:** [Vue Router](https://router.vuejs.org/)
- **UI:** [Bootstrap](https://getbootstrap.com/)
- **HTTP Kliens:** [Axios](https://axios-http.com/)
- **Validáció:** [Joi](https://joi.dev/)
- **Dátumkezelés:** [date-fns](https://date-fns.org/)

### Back-end

- **Framework:** [C# .NET](https://dotnet.microsoft.com/)
- **Adatelérés:** [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- **Validáció:** [FluentValidation](https://fluentvalidation.net/)
- **API Dokumentáció:** [Swashbuckle (Swagger)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- **Naplózás:** [Serilog](https://serilog.net/)
- **Gyorsítótárazás:** [FusionCache](https://github.com/jodydonetti/FusionCache)
- **Jelszó Hashelés:** [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net)

### Adatbázis

- **Rendszer:** Microsoft SQL Server (Express)
