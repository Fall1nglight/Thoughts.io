import Joi from 'joi'

const email = Joi.string()
  .required()
  .messages({
    'string.empty': 'Email address is required.',
  })

  .email({ tlds: { allow: false } })
  .messages({
    'string.email': 'Invalid email format.',
  })

  .max(60)
  .messages({
    'string.max': 'Email address must not exceed {{#limit}} characters.',
  })

const username = Joi.string()
  .required()
  .messages({
    'string.empty': 'Username is required.',
  })

  .min(5)
  .max(30)
  .messages({
    'string.min': 'Username must be at least {{#limit}} characters long.',
    'string.max': 'Username must not exceed {{#limit}} characters.',
  })

  .pattern(new RegExp('^[a-zA-Z0-9_]*$'))
  .messages({
    'string.pattern.base':
      'Username can only contain letters, numbers, and underscores (no spaces).',
  })

const password = Joi.string()
  .required()
  .messages({
    'string.empty': 'Password is required.',
  })

  .min(8)
  .max(60)
  .messages({
    'string.min': 'Password must be at least {{#limit}} characters long.',
    'string.max': 'Password must not exceed {{#limit}} characters.',
  })

  .pattern(new RegExp('(?=.*[A-Z])'), 'uppercase')
  .messages({
    'string.pattern.name': 'Password must contain at least one uppercase letter.',
  })

  .pattern(new RegExp('(?=.*[a-z])'), 'lowercase')
  .messages({
    'string.pattern.name': 'Password must contain at least one lowercase letter.',
  })

  .pattern(new RegExp('(?=.*[0-9])'), 'number')
  .messages({
    'string.pattern.name': 'Password must contain at least one number.',
  })

  .pattern(new RegExp('(?=.*[^a-zA-Z0-9])'), 'special')
  .messages({
    'string.pattern.name': 'Password must contain at least one special character.',
  })

export { email, username, password }
