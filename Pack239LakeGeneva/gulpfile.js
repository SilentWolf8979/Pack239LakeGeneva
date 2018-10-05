/// <binding />
var gulp = require('gulp');
var cleanCSS = require('gulp-clean-css');
var uglify = require('gulp-uglify-es').default;
var sourcemaps = require('gulp-sourcemaps');

gulp.task('copy-bootstrap-css', function () {
  return gulp.src('node_modules/bootstrap/dist/css/*.css')
    .pipe(cleanCSS({ sourceMap: true, processImport: false }))
    .pipe(gulp.dest('wwwroot/lib/css/bootstrap'));
});

gulp.task('copy-bootstrap-js', function () {
  return gulp.src('node_modules/bootstrap/dist/js/*.js')
    .pipe(sourcemaps.init())
    .pipe(uglify())
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest('wwwroot/lib/js/bootstrap'));
});

gulp.task('copy-jquery', function () {
  return gulp.src('node_modules/jquery/dist/*.js')
    .pipe(sourcemaps.init())
    .pipe(uglify())
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest('wwwroot/lib/js/jquery'));
});

gulp.task('copy-fontawesome-css', function () {
  return gulp.src('wwwroot/css/fontawesome/*.css')
    .pipe(cleanCSS({ sourceMap: true, processImport: false }))
    .pipe(gulp.dest('wwwroot/lib/css/fontawesome'));
});

gulp.task('copy-fontawesome-js', function () {
  return gulp.src('wwwroot/js/fontawesome/*.js')
    .pipe(sourcemaps.init())
    .pipe(uglify())
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest('wwwroot/lib/js/fontawesome'));
});

gulp.task('minify-site-css', function () {
  return gulp.src('wwwroot/css/*.css')
    .pipe(cleanCSS({ sourceMap: true, processImport: false }))
    .pipe(gulp.dest('wwwroot/lib/css'));
});

gulp.task('minify-site-js', function () {
  return gulp.src('wwwroot/js/*.js')
    .pipe(sourcemaps.init())
    .pipe(uglify())
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest('wwwroot/lib/js'));
});

gulp.task('default', gulp.parallel('copy-bootstrap-css', 'copy-bootstrap-js', 'copy-jquery', 'copy-fontawesome-css', 'copy-fontawesome-js', 'minify-site-css', 'minify-site-js'));