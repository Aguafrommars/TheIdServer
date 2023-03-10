(() => {
    'use strict'

    const setTheme = theme => {
        document.documentElement.setAttribute('data-bs-theme', theme)
        localStorage.setItem('theme', theme)
        const span = window.document.getElementById('theme-icon')
        span.className = theme === 'dark' ? 'oi oi-sun' : 'oi oi-moon'
    }
    const getTheme = () => {
        const storedTheme = localStorage.getItem('theme')

        if (storedTheme) {
            return storedTheme
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
    }

    const togleButton = window.document.getElementById('theme-toggle-button')
    togleButton.addEventListener('click', ev => {
        const theme = getTheme() === 'dark' ? 'light' : 'dark'
        setTheme(theme)
    })
    setTheme(getTheme())
})()
