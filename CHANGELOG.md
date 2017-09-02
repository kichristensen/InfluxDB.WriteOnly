# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Dotnet Core 2.0 support

## 2.0.0 - 2017-08-30

### Added
- FAKE build script.
- ILogger interface with default implementation write to the console.
- Implemented InfluxDbOptions class, to configure optional options.
- Better debugging experience by adding DebuggerDisplayAttribute to InfluxDbClient.
- Added unit tests to improve quality

### Changed
- All optional options is moved into the InfluxDbOptions class, only the endpoint URI remains in the client constructor.

## 1.2.3

Changes before and including this version wasn't track.