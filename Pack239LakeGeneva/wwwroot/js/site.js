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
  if ($(".calendarEvents").length > 0) {
    $(".calendarEvents").load('/Components/Calendar/Default');
  }

  if ($(".documents").length > 0) {
    if (typeof (documentId) !== undefined) {
      $(".documents").load('/Components/Resources/Default/' + documentId);
    }
    else {
      $(".documents").load('/Components/Resources/Default');
    }
  }

  //$("#imageCarousel").swiperight(function () {
  //  $(this).carousel('prev');
  //});
  //$("#imageCarousel").swipeleft(function () {
  //  $(this).carousel('next');
  //});
});

function WireCalendarEvents() {
  $('input[type=checkbox]').change(function () {
    if (!this.checked) {
      $(".calendarEvent." + this.name).css("cssText", "display: none !important;");
    }
    else {
      $(".calendarEvent." + this.name).css("cssText", "");
    }
  });
}

function ShowPackEvents() {
  $(":checkbox").click();
}