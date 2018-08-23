/// <binding AfterBuild='copy-bootstrap, copy-jquery, copy-fontawesome, minify-site' />
var gulp = require('gulp');
var cleanCSS = require('gulp-clean-css');
var rename = require('gulp-rename');
var uglify = require('gulp-uglify');
var sourcemaps = require('gulp-sourcemaps');

gulp.task('copy-bootstrap', function ()
{
  try
  {
    gulp.src('node_modules/bootstrap/dist/css/*.css')
      .pipe(cleanCSS({ sourceMap: true, processImport: false }))
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
      .pipe(sourcemaps.init())
      .pipe(uglify())
      .pipe(sourcemaps.write('./'))
      .pipe(gulp.dest('wwwroot/lib/js/jquery'));
  }
  catch (e)
  {
    return -1;
  }
  return 0;
});

gulp.task('copy-fontawesome', function () {
  try {
    gulp.src('wwwroot/css/fontawesome/*.css')
      .pipe(cleanCSS({ sourceMap: true, processImport: false }))
      .pipe(gulp.dest('wwwroot/lib/css/fontawesome'));

    gulp.src('wwwroot/js/fontawesome/*.js')
      .pipe(sourcemaps.init())
      .pipe(uglify())
      .pipe(sourcemaps.write('./'))
      .pipe(gulp.dest('wwwroot/lib/js/fontawesome'));
  }
  catch (e) {
    return -1;
  }
  return 0;
});

gulp.task('minify-site', function () {
  try {
    gulp.src('wwwroot/css/*.css')
      .pipe(cleanCSS({ sourceMap: true, processImport: false }))
      .pipe(gulp.dest('wwwroot/lib/css'));

    gulp.src('wwwroot/js/*.js')
      .pipe(sourcemaps.init())
      .pipe(uglify())
      .pipe(sourcemaps.write('./'))
      .pipe(gulp.dest('wwwroot/lib/js'));
  }
  catch (e) {
    return -1;
  }
  return 0;
});