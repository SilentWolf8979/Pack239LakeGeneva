/// <binding AfterBuild='copy-bootstrap, copy-jquery' />
var gulp = require('gulp')

gulp.task('copy-bootstrap', function ()
{
  try
  {
    gulp.src('node_modules/bootstrap/dist/css/*.css')
      .pipe(gulp.dest('wwwroot/lib/css/bootstrap'));

    gulp.src('node_modules/bootstrap/dist/js/*.js')
      .pipe(gulp.dest('wwwroot/lib/js/bootstrap'));
  }
  catch (e)
  {
    return -1;
  }
  return 0;
});

gulp.task('copy-jquery', function () {
  try
  {
    gulp.src('node_modules/jquery/dist/*.js')
      .pipe(gulp.dest('wwwroot/lib/js/jquery'));
  }
  catch (e)
  {
    return -1;
  }
  return 0;
});