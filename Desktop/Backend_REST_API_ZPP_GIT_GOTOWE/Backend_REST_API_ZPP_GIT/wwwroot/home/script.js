document.getElementById('search-form').addEventListener('submit', async function (event) {
    event.preventDefault();

    const exactGroup = document.getElementById('exactgroup').value;

    if (!exactGroup) {
        alert("Wybierz dokładną grupę!");
        return;
    }

    try {
        const response = await fetch('http://localhost:5096/api/scraper/scrape', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: new URLSearchParams({ exactGroup }) 
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error("Error:", errorText);
            throw new Error("Błąd serwera przy pobieraniu kursów!");
        }

        const data = await response.json();
        displayResults(data);
    } catch (error) {
        console.error('Błąd:', error);
    }
});

async function fetchTeachers() {
    try {
        const response = await fetch('http://localhost:5096/api/prowadzacy/get-full-name');
        if (!response.ok) throw new Error("Błąd serwera!");

        const teachers = await response.json();
        const teacherMap = new Map();

        teachers.forEach(teacher => {
            teacherMap.set(teacher.Skrot.toUpperCase(), teacher.PelneImie);
        });

        return teacherMap;
    } catch (error) {
        console.error("Błąd przy pobieraniu wykładowców:", error);
        return new Map();
    }
}

async function displayResults(data) {
    const resultsContainer = document.getElementById('results');
    resultsContainer.innerHTML = '';

    if (!data || data.length === 0) {
        resultsContainer.innerHTML = '<p>Brak wyników dla podanych kryteriów.</p>';
        return;
    }

    const teacherMap = await fetchTeachers();
    const list = document.createElement('ul');
    data.forEach(course => {
        const teacherAbbreviations = course.teacher?.split(' ') || [];
        const fullTeacherNames = teacherAbbreviations.map(abbr => teacherMap.get(abbr.toUpperCase()) || abbr);
        const fullTeacherName = fullTeacherNames.join(' ');


        const listItem = document.createElement('li');
        listItem.textContent = `${course.courseCode} - ${course.courseName} | Typ: ${course.type} | Wykładowca: ${fullTeacherName}`;
        list.appendChild(listItem);
    });

    resultsContainer.appendChild(list);
}
