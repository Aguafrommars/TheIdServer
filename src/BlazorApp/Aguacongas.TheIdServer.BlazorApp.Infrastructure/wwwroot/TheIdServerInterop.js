/* eslint-disable no-undef */
window.bootstrapInteropt = {
  showToast: (id, dotnetHelper) => {
    const query = `#${id}`
    $(query).on('hidden.bs.toast', function () {
      dotnetHelper.invokeMethodAsync('ToastClosed', id)
        .then(_ => { /* invoke it */ })
    })
    $(query).toast('show')
  },
  showModal: id => {
    $(`#${id}`).modal('show')
  },
  dismissModal: id => {
    $(`#${id}`).modal('hide')
  },
  showDropDownMenu: id => {
    $(`#${id}`).dropdown('show')
  },
  hideDropDownMenu: id => {
    $(`#${id}`).dropdown('hide')
  }
}
window.browserInteropt = {
  scrollTo: (id, offset) => {
    const element = document.getElementById(id)
    const y = element.getBoundingClientRect().top + window.scrollY
    offset = offset || 0
    window.scrollTo({ top: y + offset, behavior: 'smooth' })
  },
  preventEnterKeyPress: (id, dotnetHelper) => {
    const element = document.getElementById(id)
    element.onkeydown = e => {
      if (e.key === 'Enter') {
        e.preventDefault()
        dotnetHelper.invokeMethodAsync('EnterKeyPressed', id)
          .then(_ => { /* invoke it */ })
      }
    }
  },
  onScrollEnd: (dotnetHelper, margin) => {
    let pending = false
    window.onscroll = async () => {
      const top = $(window).scrollTop()
      const diff = $(document).height() - $(window).height() - margin
      if (top >= diff && !pending) {
        pending = true
        await dotnetHelper.invokeMethodAsync('ScrollBottomReach')
        pending = false
      }
    }
  },
  isScrollable: margin => {
    return $(document).height() > $(window).height() - margin;
  }
}
window.setTheme = theme => {
  document.documentElement.setAttribute('data-bs-theme', theme)
  localStorage.setItem('theme', theme)
}
window.getTheme = () => {
  const storedTheme = localStorage.getItem('theme')

  if (storedTheme) {
    return storedTheme
  }

  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}
setTheme(getTheme())
