// src/composables/useFormat.js
// Утилиты форматирования для всего приложения

export function useFormat() {
  /**
   * Форматирование суммы в тенге
   * @param {number} amount
   * @param {boolean} short - сокращённый формат (1.2M, 500K)
   */
  function money(amount, short = false) {
    if (amount === null || amount === undefined) return '0 ₸'
    const n = Number(amount)
    if (short) {
      if (n >= 1_000_000) return (n / 1_000_000).toFixed(1) + 'M ₸'
      if (n >= 1_000)     return (n / 1_000).toFixed(0)     + 'K ₸'
    }
    return new Intl.NumberFormat('ru-KZ', { maximumFractionDigits: 0 }).format(n) + ' ₸'
  }

  /**
   * Форматирование даты
   */
  function date(d, opts = {}) {
    if (!d) return '—'
    return new Date(d).toLocaleDateString('ru-KZ', {
      day: 'numeric', month: 'long', year: 'numeric', ...opts
    })
  }

  function dateShort(d) {
    if (!d) return '—'
    return new Date(d).toLocaleDateString('ru-KZ', { day: 'numeric', month: 'short', year: 'numeric' })
  }

  function timeAgo(d) {
    if (!d) return ''
    const diff  = Date.now() - new Date(d).getTime()
    const mins  = Math.floor(diff / 60000)
    const hours = Math.floor(diff / 3600000)
    const days  = Math.floor(diff / 86400000)
    if (mins < 1)   return 'только что'
    if (mins < 60)  return `${mins} мин. назад`
    if (hours < 24) return `${hours} ч. назад`
    if (days < 7)   return `${days} д. назад`
    return dateShort(d)
  }

  /**
   * Процент сбора
   */
  function fundPercent(collected, goal) {
    if (!goal || goal === 0) return 0
    return Math.min(100, Math.round((collected / goal) * 100))
  }

  /**
   * Обрезать строку
   */
  function truncate(str, maxLen = 100) {
    if (!str) return ''
    return str.length > maxLen ? str.slice(0, maxLen) + '…' : str
  }

  /**
   * Склонение числительных: дней/день/дня
   */
  function plural(n, one, few, many) {
    const mod10  = Math.abs(n) % 10
    const mod100 = Math.abs(n) % 100
    if (mod100 >= 11 && mod100 <= 14) return `${n} ${many}`
    if (mod10 === 1) return `${n} ${one}`
    if (mod10 >= 2 && mod10 <= 4) return `${n} ${few}`
    return `${n} ${many}`
  }

  const MONTHS = ['', 'Январь','Февраль','Март','Апрель','Май','Июнь',
                  'Июль','Август','Сентябрь','Октябрь','Ноябрь','Декабрь']
  function monthName(n) { return MONTHS[n] || '' }

  return { money, date, dateShort, timeAgo, fundPercent, truncate, plural, monthName }
}
