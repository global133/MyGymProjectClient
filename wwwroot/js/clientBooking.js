
document.addEventListener('DOMContentLoaded', function () {
    const bookingModal = document.getElementById('bookingModal');
    if (bookingModal) {
        const modalInstance = new bootstrap.Modal(bookingModal);

        bookingModal.addEventListener('show.bs.modal', async function (e) {
            const button = e.relatedTarget;
            const trainerId = button.getAttribute('data-trainer-id');
            const trainerName = button.getAttribute('data-trainer-name');

            document.getElementById('trainerId').value = trainerId;
            document.getElementById('trainerName').textContent = trainerName;

            await loadTrainerSchedule(trainerId);
        });

        document.getElementById('confirmBooking').addEventListener('click', async function () {
            const form = document.getElementById('bookingForm');
            const formData = new FormData(form);

            const trainerId = form.querySelector('#trainerId').value;  // <-- берём из скрытого input
            const slotId = form.querySelector('#bookingTime').value;   // если нужно

            try {
                const response = await fetch(`http://localhost:5155/api/trainings/${trainerId}/clients/${clientId}`, {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    alert('Запись успешно создана!');
                    modalInstance.hide();
                } else {
                    alert('Ошибка при записи');
                }
            } catch (error) {
                console.error('Error:', error);
                alert('Произошла ошибка при отправке данных');
            }
        });
    }
});

async function loadTrainerSchedule(trainerId) {
    try {
        const response = await fetch(`http://localhost:5155/api/trainings/trainer/${trainerId}`);
        console.log('HTTP Status:', response.status, response.statusText);
        if (!response.ok) throw new Error("Ошибка загрузки расписания");

        const schedule = await response.json();
        displayAvailableSlots(schedule);
    } catch (error) {
        console.error(error);
        alert("Не удалось загрузить расписание");
    }
}

function displayAvailableSlots(schedule) {
    const slotsContainer = document.getElementById('availableSlots');
    if (!slotsContainer) return;

    slotsContainer.innerHTML = schedule
        .filter(slot => !slot.isBooked)
        .map(slot => {
            const date = new Date(slot.date);
            const formattedDate = date.toLocaleDateString('ru-RU');
            return `
                <div class="time-slot mb-2 p-2 border rounded">
                    <span>${formattedDate} ${slot.startTime}-${slot.endTime}</span>
                    <button class="btn btn-sm btn-success float-end btn-book" data-slot-id="${slot.id}">Выбрать</button>
                </div>
            `;
        })
        .join('');

    // Добавляем обработчики для кнопок выбора времени
    document.querySelectorAll('.btn-book').forEach(btn => {
        btn.addEventListener('click', function () {
            const slotId = this.getAttribute('data-slot-id');
            document.getElementById('bookingTime').value = slotId;
        });
    });
}