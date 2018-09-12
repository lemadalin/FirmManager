var gulp = require('gulp');
var sass = require('gulp-sass');
var del = require('del');

gulp.task('css-clean', function () {
    return del('wwwroot/css/**/*.css', { force: true });
});

gulp.task('sass-compile', function () {
    return gulp.src('wwwroot/sass/**/*.scss')
        .pipe(sass())
        .pipe(gulp.dest('wwwroot/css'))
});

gulp.task('sass-watch', function () {
    gulp.watch('wwwroot/sass/**/*.scss', ['sass-compile']);
});