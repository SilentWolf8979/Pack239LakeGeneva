$('.dropdown').on('show.bs.dropdown', function (e) {
  $(this).find('.dropdown-menu').first().stop(true, true).slideDown({
    duration: 300,
    start: function () {
      $(this).css("display", "flex");
    }
  });
});

$('.dropdown').on('hide.bs.dropdown', function (e) {
  $(this).find('.dropdown-menu').first().stop(true, true).slideUp(200);
});

$(document).ready(function () {
  $(".calendarEvents").load('Components/Calendar/Default');
});

function WireCalendarEvents()
{
  $('input[type=checkbox]').change(function () {
    if (!this.checked) {
      $(".calendarEvent." + this.name).css("cssText", "display: none !important;");
    }
    else {
      $(".calendarEvent." + this.name).css("cssText", "");
    }
  });
}