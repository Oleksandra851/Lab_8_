class AppManager {
    constructor() {
        this.apiBase = "/api";
        this.positions = [];
        this.subjects = [];
        this.init();
    }

    async init() {
        await this.loadPositions();
        await this.loadSubjects();
        await this.loadTeachers();
        this.bindEvents();
    }

    bindEvents() {
        const btnAdd = document.getElementById("btnAddTeacher");
        if (btnAdd) {
            btnAdd.addEventListener("click", () => this.openCreateModal());
        }

        const btnSave = document.getElementById("btnSaveTeacher");
        if (btnSave) {
            btnSave.addEventListener("click", () => this.saveTeacher());
        }
    }

    async loadPositions() {
        const res = await fetch(`${this.apiBase}/positions`);
        this.positions = await res.json();

        const select = document.getElementById("positionId");
        if (select) {
            select.innerHTML = '<option value="">-- Оберіть посаду --</option>';
            this.positions.forEach(p => {
                select.innerHTML += `<option value="${p.positionID}">${p.positionName}</option>`;
            });
        }
    }

    async loadSubjects() {
        const res = await fetch(`${this.apiBase}/subjects`);
        this.subjects = await res.json();

        const tbody = document.getElementById("subjectsTableBody");
        if (tbody) {
            tbody.innerHTML = "";
            this.subjects.forEach(s => {
                tbody.innerHTML += `
                    <tr>
                        <td class="text-center">
                            <input type="checkbox" class="form-check-input subject-checkbox" data-subject-id="${s.subjectID}" />
                        </td>
                        <td><strong>${s.subjectName}</strong></td>
                        <td>
                            <input type="number" class="form-control form-control-sm subject-hours" data-subject-id="${s.subjectID}" min="0" value="0" />
                        </td>
                    </tr>
                `;
            });
        }
    }

    async loadTeachers() {
        const res = await fetch(`${this.apiBase}/teachers`);
        const teachers = await res.json();

        const tbody = document.getElementById("teachersList");
        if (!tbody) return;

        tbody.innerHTML = "";
        teachers.forEach(t => {
            let subjectsHtml = '<span class="text-muted fst-italic">Не призначено</span>';
            if (t.subjects && t.subjects.length > 0) {
                subjectsHtml = '<ul class="list-unstyled mb-0">' +
                    t.subjects.map(s => `<li>• ${s.subjectName} <span class="text-muted">(${s.hoursRead} год)</span></li>`).join('') +
                    '</ul>';
            }

            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td class="fw-bold text-primary">${t.lastName} ${t.firstName} ${t.middleName ?? ''}</td>
                <td><span class="badge bg-primary-subtle text-primary border border-primary-subtle px-2 py-1 fs-6">${t.positionName ?? ''}</span></td>
                <td>${subjectsHtml}</td>
                <td class="text-center">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-info text-white fw-semibold px-2" onclick="appManager.openDetailsModal(${t.teacherID})">Деталі</button>
                        <button class="btn btn-warning fw-semibold px-2" onclick="appManager.openEditModal(${t.teacherID})">Редагувати</button>
                        <button class="btn btn-danger fw-semibold px-2" onclick="appManager.deleteTeacher(${t.teacherID})">Видалити</button>
                    </div>
                </td>
            `;
            tbody.appendChild(tr);
        });
    }

    openCreateModal() {
        const form = document.getElementById("teacherForm");
        if (form) form.reset();

        document.getElementById("teacherId").value = "0";
        document.getElementById("modalTitle").innerText = "Додавання викладача";

        document.querySelectorAll(".subject-checkbox").forEach(cb => cb.checked = false);
        document.querySelectorAll(".subject-hours").forEach(inp => inp.value = "0");

        const modal = new bootstrap.Modal(document.getElementById("teacherModal"));
        modal.show();
    }

    async openEditModal(id) {
        const res = await fetch(`${this.apiBase}/teachers/${id}`);
        const t = await res.json();

        document.getElementById("teacherId").value = t.teacherID.toString();
        document.getElementById("lastName").value = t.lastName;
        document.getElementById("firstName").value = t.firstName;
        document.getElementById("middleName").value = t.middleName ?? "";
        document.getElementById("phone").value = t.phone ?? "";
        document.getElementById("workplace").value = t.workplace ?? "";
        document.getElementById("positionId").value = t.positionID.toString();
        document.getElementById("homeAddress").value = t.homeAddress ?? "";
        document.getElementById("characteristic").value = t.characteristic ?? "";

        document.querySelectorAll(".subject-checkbox").forEach(cb => cb.checked = false);
        document.querySelectorAll(".subject-hours").forEach(inp => inp.value = "0");

        if (t.subjects) {
            t.subjects.forEach(sub => {
                const cb = document.querySelector(`.subject-checkbox[data-subject-id='${sub.subjectID}']`);
                const inp = document.querySelector(`.subject-hours[data-subject-id='${sub.subjectID}']`);
                if (cb) cb.checked = true;
                if (inp) inp.value = sub.hoursRead.toString();
            });
        }

        document.getElementById("modalTitle").innerText = "Редагування викладача";
        const modal = new bootstrap.Modal(document.getElementById("teacherModal"));
        modal.show();
    }

    async openDetailsModal(id) {
        const res = await fetch(`${this.apiBase}/teachers/${id}`);
        const t = await res.json();

        document.getElementById("detFullName").innerText = `${t.lastName} ${t.firstName} ${t.middleName ?? ''}`;
        document.getElementById("detPhone").innerText = t.phone ?? '—';
        document.getElementById("detWorkplace").innerText = t.workplace ?? '—';
        document.getElementById("detPosition").innerText = t.positionName ?? '—';
        document.getElementById("detAddress").innerText = t.homeAddress ?? '—';
        document.getElementById("detCharacteristic").innerText = t.characteristic ?? '—';

        let subHtml = 'Нічого не призначено';
        if (t.subjects && t.subjects.length > 0) {
            subHtml = '<ul class="list-unstyled mb-0">' +
                t.subjects.map(s => `<li>• <strong>${s.subjectName}</strong> — ${s.hoursRead} год.</li>`).join('') +
                '</ul>';
        }
        document.getElementById("detSubjects").innerHTML = subHtml;

        const modal = new bootstrap.Modal(document.getElementById("detailsModal"));
        modal.show();
    }

    async saveTeacher() {
        const id = parseInt(document.getElementById("teacherId").value);
        const lastName = document.getElementById("lastName").value.trim();
        const firstName = document.getElementById("firstName").value.trim();
        const positionId = parseInt(document.getElementById("positionId").value);

        if (!lastName || !firstName || !positionId) {
            alert("Прізвище, Ім'я та Посада є обов'язковими!");
            return;
        }

        const selectedSubjects = [];
        document.querySelectorAll(".subject-checkbox").forEach(cb => {
            if (cb.checked) {
                const subjectId = parseInt(cb.getAttribute("data-subject-id"));
                const hoursInput = document.querySelector(`.subject-hours[data-subject-id='${subjectId}']`);
                const hoursRead = hoursInput ? parseInt(hoursInput.value) || 0 : 0;
                selectedSubjects.push({ subjectID: subjectId, hoursRead: hoursRead });
            }
        });

        const dto = {
            teacherID: id,
            lastName: lastName,
            firstName: firstName,
            middleName: document.getElementById("middleName").value,
            phone: document.getElementById("phone").value,
            workplace: document.getElementById("workplace").value,
            positionID: positionId,
            homeAddress: document.getElementById("homeAddress").value,
            characteristic: document.getElementById("characteristic").value,
            selectedSubjects: selectedSubjects
        };

        const method = id === 0 ? "POST" : "PUT";
        const url = id === 0 ? `${this.apiBase}/teachers` : `${this.apiBase}/teachers/${id}`;

        const res = await fetch(url, {
            method: method,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(dto)
        });

        if (res.ok) {
            const modalElement = document.getElementById("teacherModal");
            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) modal.hide();

            await this.loadTeachers();
        } else {
            alert("Помилка під час збереження даних!");
        }
    }

    async deleteTeacher(id) {
        if (confirm("Ви дійсно бажаєте видалити цього викладача?")) {
            const res = await fetch(`${this.apiBase}/teachers/${id}`, { method: "DELETE" });
            if (res.ok) {
                await this.loadTeachers();
            } else {
                alert("Помилка при видаленні!");
            }
        }
    }
}

const appManager = new AppManager();
window.appManager = appManager;