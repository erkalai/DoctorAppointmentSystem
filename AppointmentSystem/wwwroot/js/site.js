// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Patient search
    $('#patientMobile').keypress(function (e) {
        if (e.which === 13) {
            e.preventDefault();
            searchPatient();
        }
    });

    function searchPatient() {
        var mobile = $('#patientMobile').val().trim();
        if (!mobile) return;

        $.get('/Appointment/GetPatientByMobile', { mobile: mobile })
            .done(function (response) {
                if (response.success) {
                    $('#patientId').val(response.patient.id);
                    $('#patientName').val(response.patient.fullName);
                    $('#patientMobile').val(response.patient.phone);
                    console.log(response.patient.id);
                    console.log(response.patient.fullName);
                } else {
                    $('#patientError').text(response.message).show();
                    $('#patientId').val('');
                    $('#patientName').val('');
                }
            })
            .fail(function () {
                $('#patientError').text('Patient not found').show();
                $('#patientName').val('');
                $('#patientId').val('');
            });
    }

    // Disable date until doctor is selected
    $('#appointmentDate').prop('disabled', true);

    $('#doctorSelect').change(function () {
        var doctorId = $(this).val();
        $('#appointmentDate').prop('disabled', !doctorId);

        if (!doctorId) {
            $('#timeSlot').empty().append('<option>-- Select Doctor First --</option>').prop('disabled', true);
        }
    });

    $('#appointmentDate').change(function () {
        var doctorId = $('#doctorSelect').val();
        var date = $(this).val();
        if (!doctorId || !date) return;

        $('#timeSlot').empty().append('<option>Loading slots...</option>').prop('disabled', true);

        $.get('/Appointment/GetAvailableSlots', { doctorId: doctorId, date: date })
            .done(function (slots) {
                $('#timeSlot').empty();
                if (slots.length > 0) {
                    $('#timeSlot').append('<option value="">-- Select Time --</option>');
                    slots.forEach(function (slot) {
                        $('#timeSlot').append($('<option>', {
                            value: slot + ':00',
                            text: slot
                        }));
                    });
                    $('#timeSlot').prop('disabled', false);
                } else {
                    $('#timeSlot').append('<option>No available slots</option>');
                }
            })
            .fail(function () {
                $('#timeSlot').empty().append('<option>Error loading slots</option>');
            });
    });

    // Final client-side validation
    // Update form submission handler
    $('#appointmentForm').submit(function (e) {
        // Manually validate required fields
        if (!$('#patientId').val()) {
            e.preventDefault();
            $('#patientError').text('Please select a valid patient first').show();
            return false;
        }

        if (!$('#doctorSelect').val()) {
            e.preventDefault();
            $('#doctorSelect').focus();
            return false;
        }

        if (!$('#appointmentDate').val()) {
            e.preventDefault();
            $('#appointmentDate').focus();
            return false;
        }

        if (!$('#timeSlot').val()) {
            e.preventDefault();
            $('#timeSlot').focus();
            return false;
        }

        return true;
    });
});