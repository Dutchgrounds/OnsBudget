/// <binding ProjectOpened='sass:watch' />
'use strict';
const { dest, src, watch, task, parallel, series } = require( 'gulp' );
const sass = require( 'gulp-sass' )( require( 'sass' ) );
const cleanCss = require( 'gulp-clean-css' );
const sourcemaps = require( 'gulp-sourcemaps' );

const cached = require( 'gulp-cached' );

var isolatedFolders = [
    './Pages/**/*.scss',
    './Components/**/*.scss',
    './Shared/**/*.scss'
];
var sharedFolders = [
    './Styles/**/*.scss'
];

task( 'sass-shared',
    function( )
    {
        return src( './Styles/Site.scss' )
            .pipe( sourcemaps.init( ) )
            .pipe( sass( ).on( 'error', sass.logError ) )
            .pipe( cleanCss( ) )
            .pipe( sourcemaps.write( '.' ) )
            .pipe( dest( './wwwroot/css' ) );
    } );

task( 'sass-isolated',
    function( )
    {
        return src( isolatedFolders )
            .pipe( sourcemaps.init( ) )
            .pipe( sass( ).on( 'error', sass.logError ) )
            .pipe( cleanCss( ) )
            //.pipe( cached( 'sass_compile' ) )
            .pipe( sourcemaps.write( '.' ) )
            .pipe( dest( function( file )
            {
                return file.base;
            } ) );
    } );

function sassWatchShared( )
{
    return watch( sharedFolders, series( 'sass-shared' ) );
};

function sassWatchIsolated( )
{
    return watch( isolatedFolders, series( 'sass-isolated' ) );
};

task( 'default', parallel( sassWatchIsolated, sassWatchShared ) );