$('.dropdown').on('show.bs.dropdown', function (e)
{
  $(this).find('.dropdown-menu').first().stop(true, true).slideDown({
    duration: 300,
    start: function ()
    {
      $(this).css("display", "flex");
    }
  });
});

$('.dropdown').on('hide.bs.dropdown', function (e)
{
  $(this).find('.dropdown-menu').first().stop(true, true).slideUp(200);
});

$(document).ready(function ()
{
  if ($(".calendarEvents").length > 0)
  {
    $(".calendarEvents").load('/Components/Calendar/Calendars', null, function ()
    {
      $(".calendarEvents .events").load('/Components/Calendar/Events', null, function ()
      {
        WireCalendarEvents();

        if ($(".filters").css("display") === "none")
        {
          ShowPackEvents();
        }
        else
        {
          HideCouncilDistrictEvents();
        }
      });
    });
  }

  if ($(".documents").length > 0)
  {
    if (($(".documents").attr("data-documentId") !== undefined) && ($(".documents").attr("data-documentId") !== ""))
    {
      $(".documents").load('/Components/Resources/Default/' + documentId);
    }
    else
    {
      $(".documents").load('/Components/Resources/Default');
    }
  }

  if ('IntersectionObserver' in window)
  {
    const observer = new IntersectionObserver(handleIntersection, options);
    images.forEach(img =>
    {
      observer.observe(img);
    });
  }
  else
  {
    Array.from(images).forEach(image => loadImage(image));
  }
});

function WireCalendarEvents()
{
  $('input[type=checkbox]').change(function ()
  {
    if (!this.checked)
    {
      $(".calendarEvent." + this.name).css("cssText", "display: none !important;");
    }
    else
    {
      $(".calendarEvent." + this.name).css("cssText", "");
    }
  });
}

function ShowPackEvents()
{
  $(":checkbox").click();
}

function HideCouncilDistrictEvents()
{
  $(":checkbox#Council").click();
  $(":checkbox#District").click();
}



const images = document.querySelectorAll('img');

const options = {
  root: null,
  rootMargin: '50px',
  threshold: 0
};

const fetchImage = (url) =>
{
  return new Promise((resolve, reject) =>
  {
    const image = new Image();
    image.src = url;
    image.onload = resolve;
    image.onerror = reject;
  });
};

const loadImage = (image) =>
{
  const src = image.dataset.src;
  fetchImage(src).then(() =>
  {
    image.src = src;
  });
};

const handleIntersection = (entries, observer) =>
{
  entries.forEach(entry =>
  {
    if (entry.intersectionRatio > 0)
    {
      loadImage(entry.target);
    }
  });
};